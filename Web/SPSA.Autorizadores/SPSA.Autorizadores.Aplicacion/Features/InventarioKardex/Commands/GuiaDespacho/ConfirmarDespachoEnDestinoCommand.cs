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
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.GuiaDespacho
{
    /// <summary>
    /// Confirma en el DESTINO cualquier Guía de Despacho (TRANSFERENCIA o ASIGNACION_ACTIVO).
    /// - TRANSFERENCIA: pasa a DISPONIBLE (mueve ubicación, +disponible, -transito).
    /// - ASIGNACION_ACTIVO: pasa a EN_USO (mueve ubicación, -transito, no suma disponible).
    /// Opcionalmente genera una Guía de Recepción en destino para auditoría.
    /// </summary>
    public class ConfirmarDespachoEnDestinoCommand : IRequest<RespuestaComunDTO>
    {
        public long GuiaDespachoId { get; set; }
        public string NumGuiaRecepcion { get; set; }   // Para trazar el ingreso en destino (opcional pero recomendado)
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public bool GenerarGuiaRecepcion { get; set; } = true; // crea GR en destino como evidencia
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

                // === Cargar GUIA DESPACHO con detalles y series ===
                var gd = await _contexto.RepositorioGuiaDespachoCabecera
                    .Obtener(g => g.Id == request.GuiaDespachoId)
                    .Include(g => g.Detalles.Select(d => d.SerieProducto))
                    .FirstOrDefaultAsync(cancellationToken);

                if (gd == null)
                    throw new InvalidOperationException("No se encontró la guía de despacho.");

                var tipo = (gd.TipoMovimiento ?? "").ToUpper();
                var esTransf = tipo == "TRANSFERENCIA";
                var esAsign = tipo == "ASIGNACION_ACTIVO";

                if (!esTransf && !esAsign)
                    throw new InvalidOperationException("Sólo se permite confirmar TRANSFERENCIA o ASIGNACION_ACTIVO.");

                if (string.IsNullOrWhiteSpace(gd.CodEmpresaDestino) || string.IsNullOrWhiteSpace(gd.CodLocalDestino))
                    throw new InvalidOperationException("La guía de despacho no tiene destino definido.");

                // (Opcional) Evitar doble confirmación
                // if (gd.IndEstado == "RECEPCIONADA" || gd.IndEstado == "CONFIRMADA") throw ...

                // ===== Armar acumulador de stock en DESTINO =====
                var deltas = new Dictionary<string, DeltaPair>(StringComparer.OrdinalIgnoreCase);
                var usarTransito = gd.UsarTransitoDestino;

                // ===== (Opcional) Crear CABECERA de Guía de Recepción en destino =====
                GuiaRecepcionCabecera gr = null;
                if (request.GenerarGuiaRecepcion)
                {
                    gr = new GuiaRecepcionCabecera
                    {
                        NumGuia = string.IsNullOrWhiteSpace(request.NumGuiaRecepcion) ? $"REC-{gd.NumGuia}" : request.NumGuiaRecepcion.Trim(),
                        Fecha = request.Fecha.Date,
                        ProveedorRuc = null,
                        CodEmpresaDestino = gd.CodEmpresaDestino,
                        CodLocalDestino = gd.CodLocalDestino,
                        AreaGestion = gd.AreaGestion,
                        ClaseStock = gd.ClaseStock,
                        EstadoStock = gd.EstadoStock,
                        Observaciones = string.IsNullOrWhiteSpace(request.Observaciones) ? null : request.Observaciones.Trim(),
                        // Marca de procedencia: puedes usar 'S' para transf, 'A' para asignación
                        IndTransferencia = esTransf ? "S" : "A",
                        IndEstado = "REGISTRADA",
                        UsuCreacion = request.UsuCreacion,
                        FecCreacion = DateTime.Now,
                        Detalles = new List<GuiaRecepcionDetalle>()
                    };
                    _contexto.RepositorioGuiaRecepcionCabecera.Agregar(gr);
                }

                foreach (var det in gd.Detalles)
                {
                    var esSerializable = det.SerieProductoId.HasValue;
                    if (esSerializable)
                    {
                        var serie = det.SerieProducto;
                        if (serie == null)
                        {
                            serie = await _contexto.RepositorioMaeSerieProducto
                                .Obtener(s => s.Id == det.SerieProductoId.Value)
                                .FirstOrDefaultAsync(cancellationToken);
                        }
                        if (serie == null)
                            throw new InvalidOperationException($"No se encontró la serie del detalle (Id={det.Id}).");

                        // Debe venir EN_TRANSITO
                        if (serie.StkActual != 0 || !string.Equals((serie.IndEstado ?? "").ToUpper(), "EN_TRANSITO"))
                            throw new InvalidOperationException($"La serie {serie.NumSerie} no está en tránsito para confirmar.");

                        // Mover ubicación al DESTINO
                        serie.CodEmpresa = gd.CodEmpresaDestino;
                        serie.CodLocal = gd.CodLocalDestino;

                        if (esTransf)
                        {
                            // Pasa a DISPONIBLE
                            serie.IndEstado = "DISPONIBLE";
                            serie.StkActual = 1;
                            serie.FecIngreso = request.Fecha.Date;

                            // Stock: -transito, +disponible (si se usó tránsito)
                            if (usarTransito)
                                AddDelta(deltas, det.CodProducto, gd.CodEmpresaDestino, gd.CodLocalDestino, disp: +1m, trans: -1m);
                            else
                                AddDelta(deltas, det.CodProducto, gd.CodEmpresaDestino, gd.CodLocalDestino, disp: +1m, trans: 0m);
                        }
                        else // ASIGNACION_ACTIVO
                        {
                            // Pasa a EN_USO (no disponible)
                            serie.IndEstado = "EN_USO";
                            serie.StkActual = 0;
                            // Stock: sólo -transito (no suma disponible)
                            if (usarTransito)
                                AddDelta(deltas, det.CodProducto, gd.CodEmpresaDestino, gd.CodLocalDestino, disp: 0m, trans: -1m);
                        }

                        serie.UsuModifica = request.UsuCreacion;
                        serie.FecModifica = DateTime.Now;
                        _contexto.RepositorioMaeSerieProducto.Actualizar(serie);

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
                    else
                    {
                        var qty = det.Cantidad <= 0 ? 0 : det.Cantidad;

                        if (esTransf)
                        {
                            if (usarTransito)
                                AddDelta(deltas, det.CodProducto, gd.CodEmpresaDestino, gd.CodLocalDestino, disp: +qty, trans: -qty);
                            else
                                AddDelta(deltas, det.CodProducto, gd.CodEmpresaDestino, gd.CodLocalDestino, disp: +qty, trans: 0m);
                        }
                        else // ASIGNACION_ACTIVO (consumo en destino)
                        {
                            if (usarTransito)
                                AddDelta(deltas, det.CodProducto, gd.CodEmpresaDestino, gd.CodLocalDestino, disp: 0m, trans: -qty);
                        }

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
                }

                // Aplicar deltas en stock destino
                await ApplyStockDeltasAsync(deltas, request.UsuCreacion, cancellationToken);

                // Marcar GD como confirmada/recepcionada
                gd.IndEstado = esTransf ? "RECEPCIONADA" : "CONFIRMADA";
                gd.FecModifica = DateTime.Now;
                gd.UsuModifica = request.UsuCreacion;
                _contexto.RepositorioGuiaDespachoCabecera.Actualizar(gd);

                // Un solo commit
                await _contexto.GuardarCambiosAsync();

                r.Mensaje = esTransf
                    ? "Despacho confirmado: productos recepcionados y disponibles en destino."
                    : "Asignación confirmada: productos en uso en destino.";
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
