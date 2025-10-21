using System;
using System.Collections.Generic;
using AutoMapper;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Npgsql;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Data.Entity;
using System.Linq;
using Serilog;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.GuiaDespacho
{
    public class RegistrarGuiaDespachoCommand : IRequest<RespuestaComunDTO>
    {
        public GuiaDespachoCabeceraCommand Cabecera { get; set; }
        public List<GuiaDespachoDetalleCommand> Detalle { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class GuiaDespachoCabeceraCommand
    {
        public string NumGuia { get; set; }
        public DateTime Fecha { get; set; }

        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }

        public string CodEmpresaDestino { get; set; }   // requerido si TipoMovimiento == TRANSFERENCIA
        public string CodLocalDestino { get; set; }     // requerido si TipoMovimiento == TRANSFERENCIA

        /// <summary>TRANSFERENCIA | ASIGNACION_ACTIVO | BAJA</summary>
        public string TipoMovimiento { get; set; } = "TRANSFERENCIA";

        /// <summary>Si TRANSFERENCIA, sumar a stk_transito en destino.</summary>
        public bool UsarTransitoDestino { get; set; } = true;

        public string AreaGestion { get; set; }
        public string ClaseStock { get; set; }
        public string Observaciones { get; set; }

        // Auditoría
        public string UsuCreacion { get; set; }
    }

    public class GuiaDespachoDetalleCommand
    {
        public string CodProducto { get; set; }
        public string NumSerie { get; set; }    // requerido si serializable
        public decimal Cantidad { get; set; }   // = 1 si serializable; > 0 si no serializable
        public decimal CantidadConfirmada { get; set; } = 0m ;
        public string CodActivo { get; set; }
        public string StkEstado { get; set; }
        
        public string Observaciones { get; set; }
    }

    public class RegistrarGuiaDespachoHandler : IRequestHandler<RegistrarGuiaDespachoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RegistrarGuiaDespachoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(RegistrarGuiaDespachoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                if (request?.Cabecera == null)
                    throw new InvalidOperationException("Cabecera no enviada.");

                if (request.Detalle == null || request.Detalle.Count == 0)
                    throw new InvalidOperationException("Debe registrar al menos un ítem en el detalle.");

                if (string.IsNullOrWhiteSpace(request.Cabecera.NumGuia) ||
                    string.IsNullOrWhiteSpace(request.Cabecera.CodEmpresaOrigen) ||
                    string.IsNullOrWhiteSpace(request.Cabecera.CodLocalOrigen))
                {
                    throw new InvalidOperationException("Complete los campos obligatorios de la cabecera.");
                }

                var tipoMov = (request.Cabecera.TipoMovimiento ?? "TRANSFERENCIA").Trim().ToUpper();
                var esTransferencia = tipoMov == "TRANSFERENCIA";

                // === Ahora destino también es obligatorio para ASIGNACION_ACTIVO ===
                if ((esTransferencia) &&
                    (string.IsNullOrWhiteSpace(request.Cabecera.CodEmpresaDestino) ||
                     string.IsNullOrWhiteSpace(request.Cabecera.CodLocalDestino)))
                {
                    throw new InvalidOperationException("Destino (Empresa/Local) es requerido para TRANSFERENCIA y ASIGNACION_ACTIVO.");
                }

                // Validar unicidad por empresa-origen + num_guia (ajusta constraint si difiere)
                var existe = await _contexto.RepositorioGuiaDespachoCabecera
                    .Obtener(g => g.CodEmpresaOrigen == request.Cabecera.CodEmpresaOrigen
                               && g.NumGuia == request.Cabecera.NumGuia)
                    .AnyAsync(cancellationToken);

                if (existe)
                    throw new InvalidOperationException($"La guía {request.Cabecera.NumGuia} ya existe para la empresa {request.Cabecera.CodEmpresaOrigen}.");

                // ===== Construir CABECERA =====
                var cab = new GuiaDespachoCabecera
                {
                    NumGuia = request.Cabecera.NumGuia.Trim(),
                    Fecha = request.Cabecera.Fecha.Date,
                    CodEmpresaOrigen = request.Cabecera.CodEmpresaOrigen.Trim(),
                    CodLocalOrigen = request.Cabecera.CodLocalOrigen.Trim(),
                    CodEmpresaDestino = string.IsNullOrWhiteSpace(request.Cabecera.CodEmpresaDestino) ? null : request.Cabecera.CodEmpresaDestino.Trim(),
                    CodLocalDestino = string.IsNullOrWhiteSpace(request.Cabecera.CodLocalDestino) ? null : request.Cabecera.CodLocalDestino.Trim(),
                    AreaGestion = string.IsNullOrWhiteSpace(request.Cabecera.AreaGestion) ? null : request.Cabecera.AreaGestion.Trim(),
                    ClaseStock = string.IsNullOrWhiteSpace(request.Cabecera.ClaseStock) ? null : request.Cabecera.ClaseStock.Trim(),
                    Observaciones = string.IsNullOrWhiteSpace(request.Cabecera.Observaciones) ? null : request.Cabecera.Observaciones.Trim(),
                    TipoMovimiento = tipoMov,
                    UsarTransitoDestino = request.Cabecera.UsarTransitoDestino,
                    IndEstado = "PENDIENTE_CONFIRMACION",
                    UsuCreacion = request.UsuCreacion,
                    FecCreacion = DateTime.Now,
                    Detalles = new List<GuiaDespachoDetalle>()
                };

                // Evitar repetir misma serie (producto + numSerie) en el mismo request
                var setSeriePorProducto = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // ===== ACUMULADORES =====
                // Requeridos en ORIGEN para validar disponibilidad (no serializables)
                var requeridosOrigen = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase); // key: prod|emp|loc -> total a bajar

                // Deltas de stock para aplicar en una sola pasada:
                //   - Disponible en ORIGEN: negativo (salida)
                //   - Tránsito en DESTINO: positivo (solo si TRANSFERENCIA y UsarTransitoDestino)
                var stockDeltas = new Dictionary<string, DeltaPair>(StringComparer.OrdinalIgnoreCase);

                // ===== Procesar DETALLE =====
                foreach (var item in request.Detalle)
                {
                    if (string.IsNullOrWhiteSpace(item.CodProducto))
                        throw new InvalidOperationException("El producto es obligatorio en todos los ítems.");

                    var prodInfo = await _contexto.RepositorioMaeProducto
                        .Obtener(p => p.CodProducto == item.CodProducto)
                        .Select(p => new { p.CodProducto, p.IndSerializable })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (prodInfo == null)
                        throw new InvalidOperationException($"El producto {item.CodProducto} no existe.");

                    var esSerializable = (prodInfo.IndSerializable ?? "S").ToUpper() == "S";
                    Mae_SerieProducto serieParaDetalle = null;

                    if (esSerializable)
                    {
                        if (item.Cantidad != 1)
                            throw new InvalidOperationException($"El producto {item.CodProducto} es serializable: la cantidad debe ser 1.");

                        var numSerie = (item.NumSerie ?? string.Empty).Trim();
                        if (string.IsNullOrEmpty(numSerie))
                            throw new InvalidOperationException($"El producto {item.CodProducto} es serializable: debe indicar número de serie.");

                        var clave = $"{item.CodProducto}|{numSerie}";
                        if (!setSeriePorProducto.Add(clave))
                            throw new InvalidOperationException($"La serie '{numSerie}' para el producto {item.CodProducto} está repetida en el detalle.");

                        var serie = await _contexto.RepositorioMaeSerieProducto
                            .Obtener(s => s.CodProducto == item.CodProducto && s.NumSerie == numSerie)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (serie == null)
                            throw new InvalidOperationException($"La serie '{numSerie}' del producto {item.CodProducto} no existe.");

                        // Debe estar DISPONIBLE en el ORIGEN (stk_actual==1, ubicación coincide, estado DISPONIBLE)
                        if (serie.StkActual != 1 ||
                            !string.Equals(serie.CodEmpresa ?? "", cab.CodEmpresaOrigen, StringComparison.OrdinalIgnoreCase) ||
                            !string.Equals(serie.CodLocal ?? "", cab.CodLocalOrigen, StringComparison.OrdinalIgnoreCase) ||
                            !string.Equals((serie.IndEstado ?? "").ToUpper(), "DISPONIBLE", StringComparison.OrdinalIgnoreCase))
                        {
                            var ubi = $"{serie.CodEmpresa ?? ""}-{serie.CodLocal ?? ""}";
                            throw new InvalidOperationException($"La serie '{numSerie}' no está DISPONIBLE en el origen {cab.CodEmpresaOrigen}-{cab.CodLocalOrigen}. Ubicación actual: {ubi}.");
                        }

                        // Marcar salida de la serie
                        serie.StkActual = 0;
                        serie.FecSalida = cab.Fecha;
                        serie.StkEstado = item.StkEstado;

                        // IMPORTANTE: TRANSFERENCIA quedan EN_TRANSITO
                        if (esTransferencia)
                        {
                            serie.IndEstado = "EN_TRANSITO";
                        }
                        else
                        {
                            // BAJA sale del stock definitivamente
                            serie.IndEstado = "BAJA";
                        }

                        serie.UsuModifica = request.UsuCreacion;
                        serie.FecModifica = DateTime.Now;
                        _contexto.RepositorioMaeSerieProducto.Actualizar(serie);

                        serieParaDetalle = serie;

                        // Deltas: ORIGEN disponible -1; DESTINO tránsito +1 si TRANSFERENCIA
                        AddDelta(stockDeltas, item.CodProducto, cab.CodEmpresaOrigen, cab.CodLocalOrigen, disp: -1m, trans: 0m);
                        if (esTransferencia && cab.UsarTransitoDestino)
                            AddDelta(stockDeltas, item.CodProducto, cab.CodEmpresaDestino, cab.CodLocalDestino, disp: 0m, trans: +1m);
                    }
                    else
                    {
                        if (item.Cantidad <= 0)
                            throw new InvalidOperationException($"La cantidad del producto {item.CodProducto} debe ser mayor a 0.");

                        // Validación acumulada contra disponible en ORIGEN
                        var kReq = $"{item.CodProducto}|{cab.CodEmpresaOrigen}|{cab.CodLocalOrigen}";
                        if (!requeridosOrigen.ContainsKey(kReq)) requeridosOrigen[kReq] = 0m;
                        requeridosOrigen[kReq] += item.Cantidad;

                        AddDelta(stockDeltas, item.CodProducto, cab.CodEmpresaOrigen, cab.CodLocalOrigen, disp: -item.Cantidad, trans: 0m);
                        if (esTransferencia && cab.UsarTransitoDestino)
                            AddDelta(stockDeltas, item.CodProducto, cab.CodEmpresaDestino, cab.CodLocalDestino, disp: 0m, trans: +item.Cantidad);
                    }

                    // Crear DETALLE hijo
                    var det = new GuiaDespachoDetalle
                    {
                        GuiaDespacho = cab,
                        CodProducto = item.CodProducto,
                        Cantidad = item.Cantidad,
                        CantidadConfirmada = item.CantidadConfirmada,
                        StkEstado = item.StkEstado,
                        CodActivo = string.IsNullOrWhiteSpace(item.CodActivo) ? null : item.CodActivo.Trim(),
                        Observaciones = string.IsNullOrWhiteSpace(item.Observaciones) ? null : item.Observaciones.Trim()
                    };

                    if (esSerializable && serieParaDetalle != null)
                    {
                        det.SerieProducto = serieParaDetalle;
                        if (serieParaDetalle.Id > 0) det.SerieProductoId = serieParaDetalle.Id;
                    }

                    cab.Detalles.Add(det);
                }

                // ===== Validar disponible en ORIGEN (no serializables) =====
                if (requeridosOrigen.Count > 0)
                {
                    var cods = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    var emps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    var locs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var k in requeridosOrigen.Keys)
                    {
                        var parts = k.Split('|');
                        cods.Add(parts[0]); emps.Add(parts[1]); locs.Add(parts[2]);
                    }

                    var existentes = await _contexto.RepositorioStockProducto
                        .Obtener(s => cods.Contains(s.CodProducto)
                                   && emps.Contains(s.CodEmpresa)
                                   && locs.Contains(s.CodLocal))
                        .Select(s => new { s.CodProducto, s.CodEmpresa, s.CodLocal, s.StkDisponible })
                        .ToListAsync(cancellationToken);

                    var dictDisp = existentes.ToDictionary(
                        x => $"{x.CodProducto}|{x.CodEmpresa}|{x.CodLocal}",
                        x => x.StkDisponible,
                        StringComparer.OrdinalIgnoreCase);

                    foreach (var kv in requeridosOrigen)
                    {
                        dictDisp.TryGetValue(kv.Key, out var disponible);
                        if (disponible < kv.Value)
                            throw new InvalidOperationException($"Stock insuficiente en origen para {kv.Key}. Disponible: {disponible}, requerido: {kv.Value}.");
                    }
                }

                // Adjuntar CABECERA
                _contexto.RepositorioGuiaDespachoCabecera.Agregar(cab);

                // Aplicar deltas (ORIGEN disponible y, si corresponde, DESTINO tránsito)
                await ApplyStockDeltasAsync(stockDeltas, request.UsuCreacion, cancellationToken);

                // Un solo commit para todo el grafo
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Guía de despacho registrada correctamente.";
            }
            catch (DbUpdateException dbEx)
            {
                var pg = FindPostgresException(dbEx);

                if (pg != null)
                {
                    _logger.Error(dbEx,
                        "Error PG: SqlState={SqlState}, Constraint={Constraint}, Detail={Detail}",
                        pg.SqlState, pg.ConstraintName, pg.Detail);

                    switch (pg.SqlState)
                    {
                        case PostgresErrorCodes.UniqueViolation:
                            if (string.Equals(pg.ConstraintName, "ux_desp_emp_origen_num_guia", StringComparison.OrdinalIgnoreCase)
                                || string.Equals(pg.ConstraintName, "UX_GUIA_DESP_CAB_EMP_ORIG_NUM_GUIA", StringComparison.OrdinalIgnoreCase))
                            {
                                var emp = request?.Cabecera?.CodEmpresaOrigen ?? "(empresa)";
                                var guia = request?.Cabecera?.NumGuia ?? "(guía)";
                                return new RespuestaComunDTO
                                {
                                    Ok = false,
                                    Mensaje = $"Ya existe una guía de despacho con número '{guia}' en la empresa '{emp}'."
                                };
                            }
                            return new RespuestaComunDTO
                            {
                                Ok = false,
                                Mensaje = "No se pudo guardar: existen datos duplicados que violan una restricción de unicidad."
                            };

                        case PostgresErrorCodes.ForeignKeyViolation:
                            return new RespuestaComunDTO
                            {
                                Ok = false,
                                Mensaje = "No se pudo guardar: faltan datos relacionados (violación de llave foránea)."
                            };

                        case PostgresErrorCodes.CheckViolation:
                            return new RespuestaComunDTO
                            {
                                Ok = false,
                                Mensaje = "No se pudo guardar: alguno de los valores no cumple las reglas de validación."
                            };

                        default:
                            return new RespuestaComunDTO
                            {
                                Ok = false,
                                Mensaje = $"Error de base de datos ({pg.SqlState})."
                            };
                    }
                }

                _logger.Error(dbEx, "Error al guardar cambios (no-PG)");
                return new RespuestaComunDTO { Ok = false, Mensaje = "Ocurrió un error al guardar los datos." };
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }

        // ================= Helpers de stock =================

        private class DeltaPair
        {
            public string CodProducto { get; set; }
            public string CodEmpresa { get; set; }
            public string CodLocal { get; set; }
            public decimal DeltaDisponible { get; set; } // puede ser negativo (salida)
            public decimal DeltaTransito { get; set; }   // puede ser positivo (entrada a tránsito)
        }

        private static void AddDelta(Dictionary<string, DeltaPair> dict, string codProducto, string codEmpresa, string codLocal, decimal disp, decimal trans)
        {
            if (string.IsNullOrWhiteSpace(codProducto) || string.IsNullOrWhiteSpace(codEmpresa) || string.IsNullOrWhiteSpace(codLocal))
                return;

            var key = $"{codProducto}|{codEmpresa}|{codLocal}";
            if (!dict.TryGetValue(key, out var d))
            {
                d = new DeltaPair
                {
                    CodProducto = codProducto,
                    CodEmpresa = codEmpresa,
                    CodLocal = codLocal,
                    DeltaDisponible = 0m,
                    DeltaTransito = 0m
                };
                dict[key] = d;
            }
            d.DeltaDisponible += disp;
            d.DeltaTransito += trans;
        }

        private async Task ApplyStockDeltasAsync(
    Dictionary<string, DeltaPair> deltas,
    string usuario,
    CancellationToken ct)
        {
            if (deltas == null || deltas.Count == 0) return;

            // Armar sets para traer solo lo necesario
            var cods = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var emps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var locs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var d in deltas.Values)
            {
                cods.Add(d.CodProducto);
                emps.Add(d.CodEmpresa);
                locs.Add(d.CodLocal);
            }

            // Trae existentes (TRACKED) para poder sumar y actualizar
            var existentes = await _contexto.RepositorioStockProducto
                .Obtener(x => cods.Contains(x.CodProducto)
                           && emps.Contains(x.CodEmpresa)
                           && locs.Contains(x.CodLocal))
                .ToListAsync(ct);

            foreach (var kv in deltas)
            {
                var d = kv.Value;

                var sp = existentes.FirstOrDefault(x =>
                    x.CodProducto == d.CodProducto &&
                    x.CodEmpresa == d.CodEmpresa &&
                    x.CodLocal == d.CodLocal);

                if (sp == null)
                {
                    // Si el delta exige existencia (p.ej. bajar disponible o tránsito), no puedes crear con negativo
                    if (d.DeltaDisponible < 0m || d.DeltaTransito < 0m)
                        throw new InvalidOperationException($"No existe stock para {d.CodProducto} en {d.CodEmpresa}-{d.CodLocal}.");

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

                    if (sp.StkDisponible < 0 || sp.StkTransito < 0)
                        throw new InvalidOperationException($"Stock negativo para {kv.Key}.");

                    _contexto.RepositorioStockProducto.Agregar(sp);
                    existentes.Add(sp);
                }
                else
                {
                    // *** ACUMULA sobre el valor de BD ***
                    sp.StkDisponible += d.DeltaDisponible;
                    sp.StkTransito += d.DeltaTransito;

                    if (sp.StkDisponible < 0 || sp.StkTransito < 0)
                        throw new InvalidOperationException($"Stock negativo para {kv.Key}.");

                    sp.UsuModifica = usuario;
                    sp.FecModifica = DateTime.Now;

                    _contexto.RepositorioStockProducto.Actualizar(sp);
                }
            }
        }

        // ================= PG helper =================
        static PostgresException FindPostgresException(Exception ex)
        {
            while (ex != null)
            {
                if (ex is PostgresException pg) return pg;
                ex = ex.InnerException;
            }
            return null;
        }
    }
}

