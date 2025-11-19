using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
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
    public class CrearSrvVirtualCommand : IRequest<RespuestaComunDTO>
    {
        public long HostSerieId { get; set; }          // FK a SGP.srv_serie_caracteristica(serie_producto_id)
        public string Hostname { get; set; }           // requerido
        public string Ip { get; set; }                 // opcional (INET)
        public decimal? RamGb { get; set; }
        public int? VCores { get; set; }
        public string HddTotal { get; set; }
        public long? SoId { get; set; }
        public short? PlataformaId { get; set; }
        public string IndActivo { get; set; }          // 'S'/'N' (default 'S')
        public string Url { get; set; }

        public string UsuCreacion { get; set; }
    }

    public class CrearSrvVirtualHandler : IRequestHandler<CrearSrvVirtualCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public CrearSrvVirtualHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearSrvVirtualCommand r, CancellationToken ct)
        {
            try
            {
                if (r.HostSerieId <= 0) throw new InvalidOperationException("Host inválido.");
                if (string.IsNullOrWhiteSpace(r.Hostname)) throw new InvalidOperationException("Hostname de VM es obligatorio.");

                // Verificar existencia del host (FK apunta a srv_serie_caracteristica)
                var hostExiste = await _ctx.RepositorioSrvSerieCaracteristica
                    .Obtener(x => x.SerieProductoId == r.HostSerieId)
                    .AnyAsync(ct);
                //if (!hostExiste) throw new InvalidOperationException("El host físico no existe.");
                if (!hostExiste) throw new InvalidOperationException("Debe registrar las caracteríticas del servidor físico.");

                // Normalizar
                var now = DateTime.Now;
                var ip = NormalizarIp(r.Ip);
                var activo = string.IsNullOrWhiteSpace(r.IndActivo) ? "S" : r.IndActivo.Trim().ToUpper();

                var vm = new Dominio.Entidades.SrvVirtual
                {
                    HostSerieId = r.HostSerieId,
                    Hostname = r.Hostname.Trim(),
                    Ip = ip,
                    RamGb = r.RamGb,
                    VCores = r.VCores,
                    HddTotal = r.HddTotal,
                    SoId = r.SoId,
                    PlataformaId = r.PlataformaId,
                    IndActivo = (activo == "N") ? "N" : "S",
                    Url = string.IsNullOrWhiteSpace(r.Url) ? null : r.Url.Trim(),
                    UsuCreacion = r.UsuCreacion,
                    FecCreacion = now
                };

                _ctx.RepositorioSrvVirtual.Agregar(vm);
                await _ctx.GuardarCambiosAsync();

                return new RespuestaComunDTO { Ok = true, Mensaje = "VM creada correctamente." };
            }
            catch (DbUpdateException ex)
            {
                var pg = BuscarPg(ex);
                if (pg != null && pg.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    // Puede ser hostname duplicado (CI) o IP duplicada
                    return new RespuestaComunDTO { Ok = false, Mensaje = "No se pudo crear: datos duplicados (hostname/IP)." };
                }
                _log.Error(ex, "Error al crear VM");
                return new RespuestaComunDTO { Ok = false, Mensaje = "Error al crear la VM." };
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return new RespuestaComunDTO { Ok = false, Mensaje = ex.Message };
            }
        }

        private static string NormalizarIp(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
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
