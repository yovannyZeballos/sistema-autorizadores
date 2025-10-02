using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;
using Npgsql;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Servidor
{
    public class CrearSrvSerieDetCommand : IRequest<RespuestaComunDTO>
    {
        public long SerieProductoId { get; set; }   // requerido
        public short TipoId { get; set; }           // requerido
        public string Hostname { get; set; }        // requerido
        public string Ip { get; set; }              // opcional (INET)
        public decimal? RamGb { get; set; }
        public int? CpuSockets { get; set; }
        public int? CpuCores { get; set; }
        public string HddTotal { get; set; }
        public long? SoId { get; set; }
        public string HostnameBranch { get; set; }
        public string IpBranch { get; set; }
        public string HostnameFileServer { get; set; }
        public string IpFileServer { get; set; }
        public string HostnameUnicola { get; set; }
        public string IpUnicola { get; set; }
        public string EnlaceUrl { get; set; }
        public string IpIdracIlo { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class CrearSrvSerieDetHandler : IRequestHandler<CrearSrvSerieDetCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public CrearSrvSerieDetHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearSrvSerieDetCommand command, CancellationToken ct)
        {
            try
            {
                if (command.SerieProductoId <= 0) throw new InvalidOperationException("Serie de producto no válida.");
                if (string.IsNullOrWhiteSpace(command.Hostname)) throw new InvalidOperationException("Hostname es obligatorio.");
                if (command.TipoId <= 0) throw new InvalidOperationException("Tipo de servidor es obligatorio.");

                // Serie + ubicación
                var serie = await _ctx.RepositorioMaeSerieProducto
                    .Obtener(s => s.Id == command.SerieProductoId)
                    .Include(s => s.Producto) // si tienes navegación a mae_producto
                    .FirstOrDefaultAsync(ct);

                if (serie == null) throw new InvalidOperationException("No se encontró la serie indicada.");
                if (string.Equals((serie.IndEstado ?? "").ToUpper(), "DE_BAJA"))
                    throw new InvalidOperationException("La serie está de baja.");

                // Evitar duplicar registro de servidor para la misma serie
                var yaExiste = await _ctx.RepositorioSrvSerieDet
                    .Obtener(x => x.SerieProductoId == command.SerieProductoId)
                    .AnyAsync(ct);
                if (yaExiste) throw new InvalidOperationException("Ya existe un detalle de servidor para esta serie.");

                var now = DateTime.Now;

                var srv = new Dominio.Entidades.SrvSerieDet
                {
                    SerieProductoId = command.SerieProductoId,
                    TipoId = command.TipoId,
                    Hostname = command.Hostname?.Trim(),
                    Ip = string.IsNullOrWhiteSpace(command.Ip) ? null : command.Ip.Trim(),
                    RamGb = command.RamGb,
                    CpuSockets = command.CpuSockets,
                    CpuCores = command.CpuCores,
                    HddTotal = command.HddTotal,
                    SoId = command.SoId,
                    HostnameBranch = command.HostnameBranch?.Trim(),
                    IpBranch = string.IsNullOrWhiteSpace(command.IpBranch) ? null : command.IpBranch.Trim(),
                    HostnameFileserver = command.HostnameFileServer?.Trim(),
                    IpFileserver = string.IsNullOrWhiteSpace(command.IpFileServer) ? null : command.IpFileServer.Trim(),
                    HostnameUnicola = command.HostnameUnicola?.Trim(),
                    IpUnicola = string.IsNullOrWhiteSpace(command.IpUnicola) ? null : command.IpUnicola.Trim(),
                    EnlaceUrl = command.EnlaceUrl?.Trim(),
                    IpIdracIlo = string.IsNullOrWhiteSpace(command.IpIdracIlo) ? null : command.IpIdracIlo.Trim(),

                    UsuCreacion = command.UsuCreacion,
                    FecCreacion = now
                };

                _ctx.RepositorioSrvSerieDet.Agregar(srv);
                await _ctx.GuardarCambiosAsync();

                return new RespuestaComunDTO { Ok = true, Mensaje = "Servidor creado correctamente." };
            }
            catch (DbUpdateException dbEx)
            {
                var pg = FindPg(dbEx);
                if (pg != null)
                {
                    if (pg.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo crear: datos duplicados (hostname/IP)." };
                    }
                }
                _log.Error(dbEx, "Error al crear servidor");
                return new RespuestaComunDTO { Ok = false, Mensaje = "Error al crear el detalle de servidor." };
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return new RespuestaComunDTO { Ok = false, Mensaje = ex.Message };
            }
        }

        private static PostgresException FindPg(Exception ex)
        {
            while (ex != null) { if (ex is PostgresException p) return p; ex = ex.InnerException; }
            return null;
        }
    }
}
