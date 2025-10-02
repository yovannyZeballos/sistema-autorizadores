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
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Servidor
{
    public class ActualizarSrvSerieDetCommand : IRequest<RespuestaComunDTO>
    {
        public long SerieProductoId { get; set; }               // Id de srv_servidor a editar
        public short TipoId { get; set; }
        public string Hostname { get; set; }
        public string Ip { get; set; }

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
        public string UsuModifica { get; set; }
    }

    public class ActualizarSrvSerieDetHandler : IRequestHandler<ActualizarSrvSerieDetCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public ActualizarSrvSerieDetHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarSrvSerieDetCommand request, CancellationToken ct)
        {
            try
            {
                if (request.SerieProductoId <= 0) throw new InvalidOperationException("Id inválido.");
                if (string.IsNullOrWhiteSpace(request.Hostname)) throw new InvalidOperationException("Hostname es obligatorio.");
                if (request.TipoId <= 0) throw new InvalidOperationException("Tipo de servidor es obligatorio.");

                var srv = await _ctx.RepositorioSrvSerieDet
                    .Obtener(x => x.SerieProductoId == request.SerieProductoId)
                    .FirstOrDefaultAsync(ct);

                if (srv == null) throw new InvalidOperationException("No se encontró el registro.");

                srv.Ip = NormalizeIp(request.Ip);
                srv.IpBranch = NormalizeIp(request.IpBranch);
                srv.IpFileserver = NormalizeIp(request.IpFileServer);
                srv.IpUnicola = NormalizeIp(request.IpUnicola);
                srv.IpIdracIlo = NormalizeIp(request.IpIdracIlo);

                srv.TipoId = request.TipoId;
                srv.Hostname = request.Hostname?.Trim();
                //srv.Ip = string.IsNullOrWhiteSpace(request.Ip) ? null : request.Ip.Trim();

                srv.RamGb = request.RamGb;
                srv.CpuSockets = request.CpuSockets;
                srv.CpuCores = request.CpuCores;
                srv.HddTotal = request.HddTotal;

                srv.SoId = request.SoId;

                srv.HostnameBranch = request.HostnameBranch?.Trim();
                //srv.IpBranch = string.IsNullOrWhiteSpace(request.IpBranch) ? null : request.IpBranch.Trim();
                srv.HostnameFileserver = request.HostnameFileServer?.Trim();
                //srv.IpFileserver = string.IsNullOrWhiteSpace(request.IpFileServer) ? null : request.IpFileServer.Trim();
                srv.HostnameUnicola = request.HostnameUnicola?.Trim();
                //srv.IpUnicola = string.IsNullOrWhiteSpace(request.IpUnicola) ? null : request.IpUnicola.Trim();
                srv.EnlaceUrl = request.EnlaceUrl?.Trim();
                //srv.IpIdracIlo = string.IsNullOrWhiteSpace(request.IpIdracIlo) ? null : request.IpIdracIlo.Trim();
                srv.UsuModifica = request.UsuModifica;
                srv.FecModifica = DateTime.Now;



                _ctx.RepositorioSrvSerieDet.Actualizar(srv);
                await _ctx.GuardarCambiosAsync();

                return new RespuestaComunDTO { Ok = true, Mensaje = "Servidor actualizado correctamente." };
            }
            catch (DbUpdateException dbEx)
            {
                var pg = FindPg(dbEx);
                if (pg != null && pg.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo actualizar: datos duplicados (hostname/IP)." };
                }
                _log.Error(dbEx, "Error al actualizar servidor");
                return new RespuestaComunDTO { Ok = false, Mensaje = "Error al actualizar el detalle de servidor." };
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

        private static string NormalizeIp(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return System.Net.IPAddress.TryParse(input, out var ip)
                ? ip.ToString()
                : throw new InvalidOperationException($"Dirección IP inválida: {input}");
        }
    }
}
