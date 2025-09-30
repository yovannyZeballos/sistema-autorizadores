using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Npgsql;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.SerieProducto
{
    public class DarBajaSerieProductoCommand : IRequest<RespuestaComunDTO>
    {
        public long? SerieProductoId { get; set; }              // Alternativa 1
        public string CodProducto { get; set; }                 // Alternativa 2 (CodProducto + NumSerie)
        public string NumSerie { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Today;   // Fecha de baja (usada como FecSalida)
        //public string Observaciones { get; set; }               // (No se persiste en mae_serie_producto; útil para auditoría si la tienes)
        public string UsuEjecucion { get; set; }                // Usuario que ejecuta la baja
    }

    public class DarBajaSerieProductoHandler : IRequestHandler<DarBajaSerieProductoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public DarBajaSerieProductoHandler()
        {
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(DarBajaSerieProductoCommand request, CancellationToken cancellationToken)
        {
            var r = new RespuestaComunDTO { Ok = true };
            try
            {
                // ===== Validaciones de entrada
                if ((!request.SerieProductoId.HasValue || request.SerieProductoId.Value <= 0)
                    && (string.IsNullOrWhiteSpace(request.CodProducto) || string.IsNullOrWhiteSpace(request.NumSerie)))
                {
                    throw new InvalidOperationException("Debe indicar SerieProductoId o (CodProducto y NumSerie).");
                }

                // ===== Buscar la serie
                Mae_SerieProducto serie = null;
                if (request.SerieProductoId.HasValue && request.SerieProductoId.Value > 0)
                {
                    serie = await _contexto.RepositorioMaeSerieProducto
                        .Obtener(s => s.Id == request.SerieProductoId.Value)
                        .FirstOrDefaultAsync(cancellationToken);
                }
                else
                {
                    var cod = (request.CodProducto ?? "").Trim();
                    var ns = (request.NumSerie ?? "").Trim();
                    serie = await _contexto.RepositorioMaeSerieProducto
                        .Obtener(s => s.CodProducto == cod && s.NumSerie == ns)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (serie == null)
                    throw new InvalidOperationException("No se encontró la serie indicada.");

                var estado = (serie.IndEstado ?? "").ToUpperInvariant();

                // ===== Reglas de negocio previas a la baja
                if (estado == "DE_BAJA")
                    return new RespuestaComunDTO { Ok = false, Mensaje = "La serie ya se encuentra dada de baja." };

                if (estado == "EN_TRANSITO")
                    return new RespuestaComunDTO { Ok = false, Mensaje = "No se puede dar de baja una serie en tránsito." };

                // Estados típicos aceptados: DISPONIBLE, EN_USO
                // En tu modelo: DISPONIBLE suele tener stk_actual = 1; EN_USO usualmente 0.
                // La constraint nueva ck_serie_baja_stk0 exige stk_actual = 0 cuando DE_BAJA.

                // ===== Si estaba DISPONIBLE y stk_actual = 1 ⇒ descontar stock agregado (stk_disponible)
                if (estado == "DISPONIBLE" && (serie.StkActual) > 0)
                {
                    if (string.IsNullOrWhiteSpace(serie.CodEmpresa) || string.IsNullOrWhiteSpace(serie.CodLocal))
                        throw new InvalidOperationException("La serie disponible no tiene ubicación (empresa/local) para ajustar el stock.");

                    var sp = await _contexto.RepositorioStockProducto
                        .Obtener(x => x.CodProducto == serie.CodProducto
                                   && x.CodEmpresa == serie.CodEmpresa
                                   && x.CodLocal == serie.CodLocal)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (sp == null || sp.StkDisponible <= 0)
                        throw new InvalidOperationException("Stock inconsistente: no hay disponible para descontar en la ubicación actual.");

                    sp.StkDisponible -= 1;
                    if (sp.StkDisponible < 0)
                        throw new InvalidOperationException("El ajuste produciría stock negativo.");

                    sp.UsuModifica = request.UsuEjecucion;
                    sp.FecModifica = DateTime.Now;
                    _contexto.RepositorioStockProducto.Actualizar(sp);
                }

                // ===== Marcar BAJA en la serie
                serie.IndEstado = "DE_BAJA";
                serie.StkActual = 0;                       // requerido por ck_serie_baja_stk0
                serie.FecSalida = request.Fecha.Date;      // usamos la fecha de baja como “salida”
                serie.UsuModifica = request.UsuEjecucion;
                serie.FecModifica = DateTime.Now;
                _contexto.RepositorioMaeSerieProducto.Actualizar(serie);

                await _contexto.GuardarCambiosAsync();

                r.Mensaje = $"Serie {serie.NumSerie} del producto {serie.CodProducto} dada de baja correctamente.";
                return r;
            }
            catch (DbUpdateException dbEx)
            {
                var pg = FindPostgresException(dbEx);
                if (pg != null)
                {
                    _logger.Error(dbEx, "Error PG al dar baja de serie. SqlState={SqlState}, Constraint={Constraint}, Detail={Detail}",
                        pg.SqlState, pg.ConstraintName, pg.Detail);

                    // Mensajes más claros según constraint
                    if (pg.ConstraintName?.Equals("ck_serie_baja_stk0", StringComparison.OrdinalIgnoreCase) == true)
                        return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo dar de baja: la serie debe quedar con stock 0." };

                    if (pg.ConstraintName?.Equals("ck_stock_no_negativo", StringComparison.OrdinalIgnoreCase) == true)
                        return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo dar de baja: el ajuste dejó el stock en negativo." };

                    return new RespuestaComunDTO { Ok = false, Mensaje = $"Error de base de datos ({pg.SqlState})." };
                }

                _logger.Error(dbEx, "Error no-PG al dar baja de serie.");
                return new RespuestaComunDTO { Ok = false, Mensaje = "Ocurrió un error al dar de baja la serie." };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al dar baja de serie: {Mensaje}", ex.Message);
                return new RespuestaComunDTO { Ok = false, Mensaje = ex.Message };
            }
        }

        private static PostgresException FindPostgresException(Exception ex)
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
