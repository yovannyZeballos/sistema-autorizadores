using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Npgsql;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaDespacho;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.GuiaDespacho
{
    /// <summary>
    /// Confirmación (parcial o total) en destino - SOLO TRANSFERENCIA.
    /// </summary>
    public class ConfirmarDespachoEnDestinoCommand : IRequest<RespuestaComunDTO>
    {
        public long GuiaDespachoId { get; set; }
        public string NumGuiaRecepcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public bool GenerarGuiaRecepcion { get; set; } = true;
        public List<LineaConfirmacionDto> Lineas { get; set; } = new List<LineaConfirmacionDto>();
        public string UsuCreacion { get; set; }
    }

    public class ConfirmarDespachoEnDestinoHandler : IRequestHandler<ConfirmarDespachoEnDestinoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ConfirmarDespachoEnDestinoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ConfirmarDespachoEnDestinoCommand request, CancellationToken cancellationToken)
        {
            var r = new RespuestaComunDTO { Ok = true };

            try
            {
                if (request.GuiaDespachoId <= 0) 
                    throw new InvalidOperationException("Guía de despacho no válida.");

                if (request.Lineas == null || request.Lineas.Count == 0) 
                    throw new InvalidOperationException("No hay ítems a confirmar.");


                // === Cargar GUIA DESPACHO con detalles y series ===
                var gd = await _contexto.RepositorioGuiaDespachoCabecera
                    .Obtener(g => g.Id == request.GuiaDespachoId)
                    .Include(g => g.Detalles.Select(d => d.SerieProducto))
                    .FirstOrDefaultAsync(cancellationToken);

                if (gd == null)
                    throw new InvalidOperationException("No se encontró la guía de despacho.");

                var tipo = (gd.TipoMovimiento ?? "").ToUpperInvariant();
                if (tipo != "TRANSFERENCIA")
                    throw new InvalidOperationException("Sólo las TRANSFERENCIAS requieren confirmación en destino.");

                if (string.IsNullOrWhiteSpace(gd.CodEmpresaDestino) || string.IsNullOrWhiteSpace(gd.CodLocalDestino))
                    throw new InvalidOperationException("La guía de despacho no tiene destino definido.");

                // ===== agrupar líneas solicitadas por Id de detalle =====
                var lineasSolic = request.Lineas
                    .GroupBy(x => x.DespachoDetalleId)
                    .ToDictionary(g => g.Key, g => new
                    {
                        Cantidad = g.Sum(x => x.Cantidad),
                        NumSerie = g.Select(x => x.NumSerie).FirstOrDefault(),
                        CodProd = g.Select(x => x.CodProducto).FirstOrDefault()
                    });

                var usarTransito = gd.UsarTransitoDestino;

                // -------------------------------
                // FASE 1: VALIDAR Y PLANIFICAR
                // -------------------------------
                var errores = new List<string>();

                // Planes a aplicar si todo está OK
                var planSerializables = new List<(GuiaDespachoDetalle det, Mae_SerieProducto serie)>();
                var planNoSerializables = new List<(GuiaDespachoDetalle det, decimal qty)>();

                foreach (var det in gd.Detalles)
                {
                    if (!lineasSolic.TryGetValue(det.Id, out var linea)) continue; // no solicitada

                    var confirmado = det.CantidadConfirmada ?? 0m;
                    var total = det.Cantidad <= 0 ? 0m : det.Cantidad;
                    var pendiente = Math.Max(0m, total - confirmado);

                    if (pendiente <= 0m)
                    {
                        errores.Add($"El detalle {det.Id} ya no tiene pendiente por confirmar.");
                        continue;
                    }

                    var esSerializable = det.SerieProductoId.HasValue;
                    if (esSerializable)
                    {
                        // cada línea serializable representa 1 unidad
                        if (linea.Cantidad <= 0m) { errores.Add($"El detalle {det.Id} (serializable) debe confirmar 1."); continue; }
                        if (linea.Cantidad > 1m) { errores.Add($"El detalle {det.Id} (serializable) no puede confirmar más de 1."); continue; }

                        var serie = det.SerieProducto ?? await _contexto.RepositorioMaeSerieProducto
                            .Obtener(s => s.Id == det.SerieProductoId.Value)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (serie == null)
                        {
                            errores.Add($"No se encontró la serie del detalle {det.Id}.");
                            continue;
                        }

                        // Validar que esté en tránsito
                        if (serie.StkActual != 0 || !string.Equals((serie.IndEstado ?? "").ToUpperInvariant(), "EN_TRANSITO"))
                        {
                            errores.Add($"La serie {serie.NumSerie} del detalle {det.Id} no está en tránsito para confirmar.");
                            continue;
                        }

                        planSerializables.Add((det, serie));

                    }
                    else
                    {
                        // No serializable: confirmar la cantidad solicitada (cap a pendiente)
                        var reqCant = linea.Cantidad;
                        if (reqCant <= 0m)
                        {
                            errores.Add($"El detalle {det.Id}: cantidad a confirmar debe ser mayor a 0.");
                            continue;
                        }
                        if (reqCant > pendiente)
                        {
                            errores.Add($"El detalle {det.Id}: la cantidad solicitada ({reqCant}) excede el pendiente ({pendiente}).");
                            continue;
                        }

                        planNoSerializables.Add((det, reqCant));
                    }
                }

                if (errores.Count > 0)
                    throw new InvalidOperationException(string.Join(Environment.NewLine, errores));

                if (planSerializables.Count == 0 && planNoSerializables.Count == 0)
                    throw new InvalidOperationException("No se confirmó ninguna línea. Verifique cantidades pendientes/seleccionadas.");


                // -------------------------------
                // FASE 2: APLICAR CAMBIOS
                // -------------------------------

                // (Opcional) Crear CABECERA de Guía de Recepción en destino
                GuiaRecepcionCabecera gr = null;
                if (request.GenerarGuiaRecepcion)
                {
                    gr = new GuiaRecepcionCabecera
                    {
                        NumGuia = string.IsNullOrWhiteSpace(request.NumGuiaRecepcion) ? $"REC-{gd.NumGuia}" : request.NumGuiaRecepcion.Trim(),
                        Fecha = request.Fecha.Date,
                        ProveedorRuc = null,
                        CodEmpresaOrigen = gd.CodEmpresaOrigen,
                        CodLocalOrigen = gd.CodLocalOrigen,
                        CodEmpresaDestino = gd.CodEmpresaDestino,
                        CodLocalDestino = gd.CodLocalDestino,
                        AreaGestion = gd.AreaGestion,
                        ClaseStock = gd.ClaseStock,
                        EstadoStock = gd.EstadoStock,
                        Observaciones = string.IsNullOrWhiteSpace(request.Observaciones) ? null : request.Observaciones.Trim(),
                        IndTransferencia = "S",
                        IndEstado = "REGISTRADA",
                        UsuCreacion = request.UsuCreacion,
                        FecCreacion = DateTime.Now,
                        Detalles = new List<GuiaRecepcionDetalle>()
                    };
                    _contexto.RepositorioGuiaRecepcionCabecera.Agregar(gr);
                }

                // Acumulador de stock destino
                var deltas = new Dictionary<string, DeltaPair>(StringComparer.OrdinalIgnoreCase);

                // Serializables: 1 por línea
                foreach (var (det, serie) in planSerializables)
                {
                    // mover al DESTINO -> DISPONIBLE (entra)
                    serie.CodEmpresa = gd.CodEmpresaDestino;
                    serie.CodLocal = gd.CodLocalDestino;
                    serie.IndEstado = "DISPONIBLE";
                    serie.StkActual = 1;
                    serie.FecIngreso = request.Fecha.Date;
                    serie.UsuModifica = request.UsuCreacion;
                    serie.FecModifica = DateTime.Now;
                    _contexto.RepositorioMaeSerieProducto.Actualizar(serie);

                    // stock destino: +1 disp, -1 trans (si se usó tránsito)
                    AddDelta(deltas, det.CodProducto, gd.CodEmpresaDestino, gd.CodLocalDestino, disp: +1m, trans: usarTransito ? -1m : 0m);

                    // marcar confirmado
                    det.CantidadConfirmada = det.CantidadConfirmada + 1m;
                    _contexto.RepositorioGuiaDespachoDetalle.Actualizar(det);

                    // GR
                    if (gr != null)
                    {
                        gr.Detalles.Add(new GuiaRecepcionDetalle
                        {
                            GuiaRecepcion = gr,
                            CodProducto = det.CodProducto,
                            SerieProductoId = serie.Id,
                            Cantidad = 1,
                            CodActivo = det.CodActivo,
                            Observaciones = det.Observaciones
                        });
                    }
                }

                // No serializables: sumar qty solicitada
                foreach (var (det, qty) in planNoSerializables)
                {
                    AddDelta(deltas, det.CodProducto, gd.CodEmpresaDestino, gd.CodLocalDestino, disp: +qty, trans: usarTransito ? -qty : 0m);

                    det.CantidadConfirmada = det.CantidadConfirmada + qty;
                    _contexto.RepositorioGuiaDespachoDetalle.Actualizar(det);

                    if (gr != null)
                    {
                        gr.Detalles.Add(new GuiaRecepcionDetalle
                        {
                            GuiaRecepcion = gr,
                            CodProducto = det.CodProducto,
                            SerieProductoId = null,
                            Cantidad = qty,
                            CodActivo = det.CodActivo,
                            Observaciones = det.Observaciones
                        });
                    }
                }

                // Aplicar deltas en stock destino
                await ApplyStockDeltasAsync(deltas, request.UsuCreacion, cancellationToken);

                // Estado de la guía (total o parcial)
                bool todoConfirmado = gd.Detalles.All(d =>
                {
                    var conf = d.CantidadConfirmada;     // decimal
                    var tot = d.Cantidad <= 0 ? 0m : d.Cantidad;
                    return conf >= tot;
                });

                gd.IndEstado = todoConfirmado ? "RECEPCIONADA" : "PENDIENTE_CONFIRMACION";
                gd.FecModifica = DateTime.Now;
                gd.UsuModifica = request.UsuCreacion;
                _contexto.RepositorioGuiaDespachoCabecera.Actualizar(gd);

                // Un solo commit
                await _contexto.GuardarCambiosAsync();

                r.Mensaje = todoConfirmado
                    ? "Transferencia confirmada totalmente."
                    : "Transferencia confirmada parcialmente.";
                return r;

            }
            catch (DbUpdateException dbEx)
            {
                var pg = FindPostgresException(dbEx);
                if (pg != null)
                {
                    _logger.Error(dbEx, "Error PG: SqlState={SqlState}, Constraint={Constraint}, Detail={Detail}", pg.SqlState, pg.ConstraintName, pg.Detail);

                    switch (pg.SqlState)
                    {
                        case PostgresErrorCodes.UniqueViolation:
                            return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo confirmar: violación de unicidad." };
                        case PostgresErrorCodes.ForeignKeyViolation:
                            return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo confirmar: violación de llave foránea." };
                        case PostgresErrorCodes.CheckViolation:
                            return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo confirmar: datos no cumplen reglas de validación." };
                        default:
                            return new RespuestaComunDTO { Ok = false, Mensaje = $"Error de base de datos ({pg.SqlState})." };
                    }
                }

                _logger.Error(dbEx, "Error al confirmar despacho (no-PG)");
                return new RespuestaComunDTO { Ok = false, Mensaje = "Ocurrió un error al confirmar el despacho." };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new RespuestaComunDTO { Ok = false, Mensaje = ex.Message };
            }
        }

        // ===== helpers =====
        private class DeltaPair
        {
            public string CodProducto { get; set; }
            public string CodEmpresa { get; set; }
            public string CodLocal { get; set; }
            public decimal DeltaDisponible { get; set; }
            public decimal DeltaTransito { get; set; }
        }

        private static void AddDelta(Dictionary<string, DeltaPair> dict, string p, string e, string l, decimal disp, decimal trans)
        {
            var key = $"{p}|{e}|{l}";
            if (!dict.TryGetValue(key, out var d))
            {
                d = new DeltaPair { CodProducto = p, CodEmpresa = e, CodLocal = l, DeltaDisponible = 0m, DeltaTransito = 0m };
                dict[key] = d;
            }
            d.DeltaDisponible += disp;
            d.DeltaTransito += trans;
        }

        private async Task ApplyStockDeltasAsync(Dictionary<string, DeltaPair> deltas, string usuario, CancellationToken ct)
        {
            if (deltas == null || deltas.Count == 0) return;

            var cods = new HashSet<string>(deltas.Values.Select(x => x.CodProducto), StringComparer.OrdinalIgnoreCase);
            var emps = new HashSet<string>(deltas.Values.Select(x => x.CodEmpresa), StringComparer.OrdinalIgnoreCase);
            var locs = new HashSet<string>(deltas.Values.Select(x => x.CodLocal), StringComparer.OrdinalIgnoreCase);

            var existentes = await _contexto.RepositorioStockProducto
                .Obtener(x => cods.Contains(x.CodProducto) && emps.Contains(x.CodEmpresa) && locs.Contains(x.CodLocal))
                .ToListAsync(ct);

            foreach (var kv in deltas)
            {
                var d = kv.Value;
                var sp = existentes.FirstOrDefault(x => x.CodProducto == d.CodProducto && x.CodEmpresa == d.CodEmpresa && x.CodLocal == d.CodLocal);

                if (sp == null)
                {
                    sp = new StockProducto
                    {
                        CodProducto = d.CodProducto,
                        CodEmpresa = d.CodEmpresa,
                        CodLocal = d.CodLocal,
                        StkDisponible = d.DeltaDisponible,
                        StkReservado = 0,
                        StkTransito = d.DeltaTransito,
                        UsuModifica = usuario,
                        FecModifica = DateTime.Now
                    };
                    if (sp.StkDisponible < 0 || sp.StkTransito < 0) throw new InvalidOperationException($"Stock negativo para {kv.Key}.");
                    _contexto.RepositorioStockProducto.Agregar(sp);
                    existentes.Add(sp);
                }
                else
                {
                    sp.StkDisponible += d.DeltaDisponible;
                    sp.StkTransito += d.DeltaTransito;
                    if (sp.StkDisponible < 0 || sp.StkTransito < 0) throw new InvalidOperationException($"Stock negativo para {kv.Key}.");
                    sp.UsuModifica = usuario;
                    sp.FecModifica = DateTime.Now;
                    _contexto.RepositorioStockProducto.Actualizar(sp);
                }
            }
        }

        static PostgresException FindPostgresException(Exception ex)
        {
            while (ex != null) { if (ex is PostgresException pg) return pg; ex = ex.InnerException; }
            return null;
        }
    }
}
