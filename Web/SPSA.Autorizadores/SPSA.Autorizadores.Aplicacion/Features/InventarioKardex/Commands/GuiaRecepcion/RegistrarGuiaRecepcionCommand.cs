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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.GuiaRecepcion
{
    public class RegistrarGuiaRecepcionCommand : IRequest<RespuestaComunDTO>
    {
        public GuiaRecepcionCabeceraCommand Cabecera { get; set; }
        public List<GuiaRecepcionDetalleCommand> Detalle { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class GuiaRecepcionCabeceraCommand
    {
        public string NumGuia { get; set; }
        public string OrdenCompra { get; set; }
        public DateTime Fecha { get; set; }
        public string ProveedorRuc { get; set; }
        public string CodEmpresaOrigen { get; set; }
        public string CodLocalOrigen { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }
        public string AreaGestion { get; set; }
        public string ClaseStock { get; set; }
        public string EstadoStock { get; set; }
        public string Observaciones { get; set; }
        public string IndTransferencia { get; set; }

        // Auditoría
        public string UsuCreacion { get; set; }
    }

    public class GuiaRecepcionDetalleCommand
    {
        public string CodProducto { get; set; }
        public long? SerieProductoId { get; set; }          // si eligen serie existente
        public string NumSerie { get; set; }                // si crean serie nueva
        public decimal Cantidad { get; set; }
        public string CodActivo { get; set; }
        public string Observaciones { get; set; }
    }

    public class RegistrarGuiaRecepcionHandler : IRequestHandler<RegistrarGuiaRecepcionCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RegistrarGuiaRecepcionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(RegistrarGuiaRecepcionCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                if (request?.Cabecera == null)
                    throw new InvalidOperationException($"Cabecera no enviada.");

                if (request.Detalle == null || request.Detalle.Count == 0)
                    throw new InvalidOperationException($"Debe registrar al menos un ítem en el detalle.");

                if(string.IsNullOrWhiteSpace(request.Cabecera.NumGuia) ||
                    string.IsNullOrWhiteSpace(request.Cabecera.CodEmpresaDestino) ||
                    string.IsNullOrWhiteSpace(request.Cabecera.CodLocalDestino) ||
                    string.IsNullOrWhiteSpace(request.Cabecera.ProveedorRuc))
                {
                    throw new InvalidOperationException($"Complete los campos obligatorios de la cabecera.");
                }

                var cab = new GuiaRecepcionCabecera
                {
                    NumGuia = request.Cabecera.NumGuia.Trim(),
                    OrdenCompra = string.IsNullOrWhiteSpace(request.Cabecera.OrdenCompra) ? null : request.Cabecera.OrdenCompra.Trim(),
                    Fecha = request.Cabecera.Fecha.Date,
                    ProveedorRuc = string.IsNullOrWhiteSpace(request.Cabecera.ProveedorRuc) ? null : request.Cabecera.ProveedorRuc.Trim(),
                    CodEmpresaOrigen = request.Cabecera.CodEmpresaOrigen.Trim(),
                    CodLocalOrigen = request.Cabecera.CodLocalOrigen.Trim(),
                    CodEmpresaDestino = request.Cabecera.CodEmpresaDestino.Trim(),
                    CodLocalDestino = request.Cabecera.CodLocalDestino.Trim(),
                    AreaGestion = string.IsNullOrWhiteSpace(request.Cabecera.AreaGestion) ? null : request.Cabecera.AreaGestion.Trim(),
                    ClaseStock = string.IsNullOrWhiteSpace(request.Cabecera.ClaseStock) ? null : request.Cabecera.ClaseStock.Trim(),
                    EstadoStock = string.IsNullOrWhiteSpace(request.Cabecera.EstadoStock) ? null : request.Cabecera.EstadoStock.Trim(),
                    Observaciones = string.IsNullOrWhiteSpace(request.Cabecera.Observaciones) ? null : request.Cabecera.Observaciones.Trim(),
                    IndTransferencia = request.Cabecera.IndTransferencia,
                    IndEstado = "REGISTRADA",
                    UsuCreacion = request.UsuCreacion,
                    FecCreacion = DateTime.Now,
                    Detalles = new List<GuiaRecepcionDetalle>()
                };

                // para evitar duplicidades en el mismo request
                var setSeriePorProducto = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // ===== ACUMULADOR de stock para evitar PK duplicada =====
                var stockDeltas = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

                // ===== Procesar DETALLE =====
                foreach (var item in request.Detalle)
                {
                    if (string.IsNullOrWhiteSpace(item.CodProducto))
                        throw new InvalidOperationException("El producto es obligatorio en todos los ítems.");

                    // ¿Producto serializable?
                    var prodInfo = await _contexto.RepositorioMaeProducto
                        .Obtener(p => p.CodProducto == item.CodProducto)
                        .Select(p => new { p.CodProducto, p.IndSerializable })
                        .FirstOrDefaultAsync();

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

                        // evita repetir misma serie en el mismo payload
                        var clave = $"{item.CodProducto}|{numSerie}";

                        if (!setSeriePorProducto.Add(clave))
                            throw new InvalidOperationException($"La serie '{numSerie}' para el producto {item.CodProducto} está repetida en el detalle.");

                        // busca si ya existe (cod_producto + num_serie)
                        var serie = await _contexto.RepositorioMaeSerieProducto
                            .Obtener(s => s.CodProducto == item.CodProducto && s.NumSerie == numSerie)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (serie == null)
                        {
                            // No existe -> crearla en el local de destino (entra por primera vez)
                            serie = new Mae_SerieProducto
                            {
                                CodProducto = item.CodProducto,
                                NumSerie = numSerie,
                                IndEstado = "DISPONIBLE",
                                CodEmpresa = cab.CodEmpresaDestino,
                                CodLocal = cab.CodLocalDestino,
                                FecIngreso = cab.Fecha,
                                StkActual = 1,
                                UsuCreacion = request.UsuCreacion,
                                FecCreacion = DateTime.Now
                            };
                            _contexto.RepositorioMaeSerieProducto.Agregar(serie);

                            // Stock resumen +1
                            AddStockDelta(stockDeltas, item.CodProducto, cab.CodEmpresaDestino, cab.CodLocalDestino, 1m);
                        }
                        else
                        {
                            // Existe -> validar estado de stock
                            // Si ya está en stock (1), no permitir nueva recepción.
                            if (serie.StkActual >= 1)
                            {
                                var ubi = $"{(serie.CodEmpresa ?? "")}-{(serie.CodLocal ?? "")}";
                                throw new InvalidOperationException(
                                    $"La serie '{numSerie}' del producto {item.CodProducto} ya se encuentra en stock (ubicación: {ubi}). " +
                                    $"Debe registrarse una guía de despacho antes de volver a recepcionarla.");
                            }

                            // Si StkActual == 0, sí se permite la recepción (reingreso)
                            serie.CodEmpresa = cab.CodEmpresaDestino;
                            serie.CodLocal = cab.CodLocalDestino;
                            serie.IndEstado = "DISPONIBLE";
                            if (serie.FecIngreso == null) serie.FecIngreso = cab.Fecha;
                            serie.StkActual = 1;
                            serie.UsuModifica = request.UsuCreacion;
                            serie.FecModifica = DateTime.Now;

                            _contexto.RepositorioMaeSerieProducto.Actualizar(serie);

                            // Stock resumen +1
                            AddStockDelta(stockDeltas, item.CodProducto, cab.CodEmpresaDestino, cab.CodLocalDestino, 1m);
                        }

                        // Enlazar la serie al DETALLE
                        serieParaDetalle = serie;

                    }
                    else
                    {
                        // No serializable: cantidad > 0
                        if (item.Cantidad <= 0)
                            throw new InvalidOperationException($"La cantidad del producto {item.CodProducto} debe ser mayor a 0.");

                        // no serializable: acumula cantidad
                        AddStockDelta(stockDeltas, item.CodProducto, cab.CodEmpresaDestino, cab.CodLocalDestino, item.Cantidad);
                    }

                    // Crea el DETALLE y cuélgalo como hijo (sin usar Ids)
                    var det = new GuiaRecepcionDetalle
                    {
                        GuiaRecepcion = cab,
                        CodProducto = item.CodProducto,
                        Cantidad = item.Cantidad,
                        CodActivo = string.IsNullOrWhiteSpace(item.CodActivo) ? null : item.CodActivo.Trim(),
                        Observaciones = string.IsNullOrWhiteSpace(item.Observaciones) ? null : item.Observaciones.Trim()
                    };

                    // Vincular serie (existente o nueva)
                    if (esSerializable && serieParaDetalle != null)
                    {
                        det.SerieProducto = serieParaDetalle;                  // navegación
                        if (serieParaDetalle.Id > 0) det.SerieProductoId = serieParaDetalle.Id; // existente
                        // si es nueva, Id=0 aquí; EF resolverá FK al guardar
                    }

                    cab.Detalles.Add(det); // << hijo del agregado
                }

                // Adjunta CABECERA al contexto
                _contexto.RepositorioGuiaRecepcionCabecera.Agregar(cab);

                // >>> APLICA TODOS LOS DELTAS DE STOCK EN UNA SOLA PASADA <<<
                await ApplyStockDeltasAsync(stockDeltas, request.UsuCreacion, cancellationToken);


                // Un solo commit para todo el grafo (cab + detalles + series + stock)
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Guía de recepción registrada correctamente.";
            }
            catch (DbUpdateException dbEx)
            {
                // Busca Npgsql.PostgresException en toda la cadena de InnerException
                var pg = FindPostgresException(dbEx);

                if (pg != null)
                {
                    // Log técnico con detalles útiles
                    _logger.Error(dbEx,
                        "Error PG: SqlState={SqlState}, Constraint={Constraint}, Detail={Detail}",
                        pg.SqlState, pg.ConstraintName, pg.Detail);

                    // Mapeo por código y/o constraint
                    switch (pg.SqlState)
                    {
                        case PostgresErrorCodes.UniqueViolation: // "23505"
                            if (string.Equals(pg.ConstraintName, "ux_rec_emp_prov_num", StringComparison.OrdinalIgnoreCase))
                            {
                                // Usa datos de la request para un mensaje entendible
                                var emp = request?.Cabecera?.CodEmpresaDestino ?? "(empresa)";
                                var prov = request?.Cabecera?.ProveedorRuc ?? "(proveedor)";
                                var guia = request?.Cabecera?.NumGuia ?? "(guía)";

                                respuesta.Mensaje = $"Ya existe una guía de recepción con número '{guia}' " +
                                                    $"para el proveedor '{prov}' en la empresa '{emp}'.";
                            }
                            else
                            {
                                respuesta.Mensaje = "No se pudo guardar: existen datos duplicados que violan una restricción de unicidad.";
                            }
                            break;

                        case PostgresErrorCodes.ForeignKeyViolation: // "23503"
                            respuesta.Mensaje = "No se pudo guardar: faltan datos relacionados (violación de llave foránea).";
                            break;

                        case PostgresErrorCodes.CheckViolation: // "23514"
                            respuesta.Mensaje = "No se pudo guardar: alguno de los valores no cumple las reglas de validación.";
                            break;

                        default:
                            // Mensaje genérico pero amigable; guarda el detalle en el log
                            respuesta.Mensaje = $"Error de base de datos ({pg.SqlState}).";
                            break;
                    }

                    respuesta.Ok = false;
                    return respuesta;
                }

                // Si no es PostgresException, mensaje genérico
                _logger.Error(dbEx, "Error al guardar cambios (no-PG)");
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al guardar los datos.";
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }

        private static void AddStockDelta(Dictionary<string, decimal> dict, string codProducto, string codEmpresa, string codLocal, decimal delta)
        {
            var key = $"{codProducto}|{codEmpresa}|{codLocal}";
            if (dict.ContainsKey(key)) dict[key] += delta;
            else dict[key] = delta;
        }

        private async Task ApplyStockDeltasAsync(Dictionary<string, decimal> deltas, string usuario, CancellationToken ct)
        {
            if (deltas == null || deltas.Count == 0) return;

            // armar sets para 1 solo query de existentes
            var cods = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var emps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var locs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var k in deltas.Keys)
            {
                var parts = k.Split('|');
                cods.Add(parts[0]);
                emps.Add(parts[1]);
                locs.Add(parts[2]);
            }

            var existentes = await _contexto.RepositorioStockProducto
                .Obtener(x => cods.Contains(x.CodProducto)
                           && emps.Contains(x.CodEmpresa)
                           && locs.Contains(x.CodLocal))
                .ToListAsync(ct);

            foreach (var kv in deltas)
            {
                var parts = kv.Key.Split('|');
                var cod = parts[0];
                var emp = parts[1];
                var loc = parts[2];
                var delt = kv.Value;

                var sp = existentes.FirstOrDefault(x =>
                    x.CodProducto == cod && x.CodEmpresa == emp && x.CodLocal == loc);

                if (sp == null)
                {
                    sp = new StockProducto
                    {
                        CodProducto = cod,
                        CodEmpresa = emp,
                        CodLocal = loc,
                        StkDisponible = delt,
                        StkReservado = 0,
                        StkTransito = 0,
                        UsuModifica = usuario,
                        FecModifica = DateTime.Now
                    };
                    _contexto.RepositorioStockProducto.Agregar(sp);
                    existentes.Add(sp); // para siguientes iteraciones del mismo lote
                }
                else
                {
                    sp.StkDisponible += delt;
                    sp.UsuModifica = usuario;
                    sp.FecModifica = DateTime.Now;
                    _contexto.RepositorioStockProducto.Actualizar(sp);
                }
            }
        }

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
