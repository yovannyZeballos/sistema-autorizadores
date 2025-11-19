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
    public class ActualizarSrvVirtualCommand : IRequest<RespuestaComunDTO>
    {
        public long Id { get; set; }                   // PK de la VM
        public long HostSerieId { get; set; }          // por coherencia en edición (no cambiamos host)
        public string Hostname { get; set; }
        public string Ip { get; set; }
        public decimal? RamGb { get; set; }
        public int? VCores { get; set; }
        public string HddTotal { get; set; }
        public long? SoId { get; set; }
        public short? PlataformaId { get; set; }
        public string IndActivo { get; set; }          // 'S'/'N'
        public string Url { get; set; }

        public string UsuModifica { get; set; }
    }

    public class ActualizarSrvVirtualHandler : IRequestHandler<ActualizarSrvVirtualCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public ActualizarSrvVirtualHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarSrvVirtualCommand r, CancellationToken ct)
        {
            try
            {
                if (r.Id <= 0) throw new InvalidOperationException("Id de VM inválido.");
                if (string.IsNullOrWhiteSpace(r.Hostname)) throw new InvalidOperationException("Hostname de VM es obligatorio.");

                var vm = await _ctx.RepositorioSrvVirtual
                    .Obtener(x => x.Id == r.Id)
                    .FirstOrDefaultAsync(ct);
                if (vm == null) throw new InvalidOperationException("No se encontró la VM.");

                // Normalizar
                vm.Hostname = r.Hostname.Trim();
                vm.Ip = string.IsNullOrWhiteSpace(r.Ip) ? null : CrearIp(r.Ip);
                vm.RamGb = r.RamGb;
                vm.VCores = r.VCores;
                vm.HddTotal = r.HddTotal;
                vm.SoId = r.SoId;
                vm.PlataformaId = r.PlataformaId;
                vm.IndActivo = string.IsNullOrWhiteSpace(r.IndActivo) ? vm.IndActivo : r.IndActivo.Trim().ToUpper() == "N" ? "N" : "S";
                vm.Url = string.IsNullOrWhiteSpace(r.Url) ? null : r.Url.Trim();
                vm.UsuModifica = r.UsuModifica;
                vm.FecModifica = DateTime.Now;

                _ctx.RepositorioSrvVirtual.Actualizar(vm);
                await _ctx.GuardarCambiosAsync();

                return new RespuestaComunDTO { Ok = true, Mensaje = "VM actualizada correctamente." };
            }
            catch (DbUpdateException ex)
            {
                var pg = BuscarPg(ex);
                if (pg != null && pg.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo actualizar: datos duplicados (hostname/IP)." };
                }
                _log.Error(ex, "Error al actualizar VM");
                return new RespuestaComunDTO { Ok = false, Mensaje = "Error al actualizar la VM." };
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return new RespuestaComunDTO { Ok = false, Mensaje = ex.Message };
            }
        }

        private static string CrearIp(string valor)
        {
            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(valor.Trim(), out ip)) return ip.ToString();
            throw new InvalidOperationException($"Dirección IP inválida: {valor}");
        }

        private static PostgresException BuscarPg(Exception ex)
        {
            while (ex != null) { if (ex is PostgresException p) return p; ex = ex.InnerException; }
            return null;
        }
    }
}
