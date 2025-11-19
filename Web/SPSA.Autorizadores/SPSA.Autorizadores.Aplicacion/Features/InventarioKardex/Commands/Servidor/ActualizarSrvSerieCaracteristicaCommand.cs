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
    public class ActualizarSrvSerieCaracteristicaCommand : IRequest<RespuestaComunDTO>
    {
        public long SerieProductoId { get; set; }               // Id de srv_servidor a editar
        public short TipoId { get; set; }
        public string Hostname { get; set; }
        public string IpSo { get; set; }

        public decimal? RamGb { get; set; }
        public int? CpuSockets { get; set; }
        public int? CpuCores { get; set; }
        public string HddTotal { get; set; }

        public long? SoId { get; set; }

        public string MacAddress { get; set; }
        public DateTime FecIngreso { get; set; }
        public int? Antiguedad { get; set; }
        public string ConexionRemota { get; set; }
        public string IpRemota { get; set; }
        public string UsuModifica { get; set; }
    }

    public class ActualizarSrvSerieCaracteristicaHandler : IRequestHandler<ActualizarSrvSerieCaracteristicaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public ActualizarSrvSerieCaracteristicaHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarSrvSerieCaracteristicaCommand request, CancellationToken ct)
        {
            try
            {
                // -------- Validaciones de entrada
                if (request.SerieProductoId <= 0) throw new InvalidOperationException("Id inválido.");
                if (string.IsNullOrWhiteSpace(request.Hostname)) throw new InvalidOperationException("Hostname es obligatorio.");
                if (request.TipoId <= 0) throw new InvalidOperationException("Tipo de servidor es obligatorio.");
                if (request.Antiguedad.HasValue && request.Antiguedad.Value < 0)
                    throw new InvalidOperationException("Antigüedad no puede ser negativa.");
                if (!string.IsNullOrEmpty(request.ConexionRemota) && request.ConexionRemota.Trim().Length > 20)
                    throw new InvalidOperationException("Conexión remota excede el máximo de 20 caracteres.");

                // -------- Buscar registro existente (1:1 por serie)
                var srv = await _ctx.RepositorioSrvSerieCaracteristica
                    .Obtener(x => x.SerieProductoId == request.SerieProductoId)
                    .FirstOrDefaultAsync(ct);

                if (srv == null) throw new InvalidOperationException("No se encontró el registro.");

                // -------- Normalizaciones mínimas (DB hará el resto: índices/constraints)
                srv.TipoId = request.TipoId;
                srv.Hostname = request.Hostname.Trim();

                // INET: dejamos que Postgres valide (acepta IP y CIDR)
                srv.IpSo = string.IsNullOrWhiteSpace(request.IpSo) ? null : request.IpSo.Trim();
                srv.IpRemota = string.IsNullOrWhiteSpace(request.IpRemota) ? null : request.IpRemota.Trim();

                srv.RamGb = request.RamGb;
                srv.CpuSockets = request.CpuSockets;
                srv.CpuCores = request.CpuCores;
                srv.HddTotal = request.HddTotal;
                srv.SoId = request.SoId;

                srv.MacAddress = string.IsNullOrWhiteSpace(request.MacAddress) ? null : request.MacAddress.Trim();
                srv.FecIngreso = request.FecIngreso;            // DDL permite NULL
                srv.Antiguedad = request.Antiguedad ?? 0;
                srv.ConexionRemota = string.IsNullOrWhiteSpace(request.ConexionRemota) ? null : request.ConexionRemota.Trim();

                srv.UsuModifica = string.IsNullOrWhiteSpace(request.UsuModifica) ? "system" : request.UsuModifica.Trim();
                srv.FecModifica = DateTime.Now;

                _ctx.RepositorioSrvSerieCaracteristica.Actualizar(srv);
                await _ctx.GuardarCambiosAsync();

                return new RespuestaComunDTO { Ok = true, Mensaje = "Servidor actualizado correctamente." };
            }
            catch (DbUpdateException dbEx)
            {
                var pg = FindPg(dbEx);
                if (pg != null)
                {
                    // 22P02: invalid_text_representation (INET inválido, etc.)
                    if (pg.SqlState == "22P02")
                    {
                        return new RespuestaComunDTO { Ok = false, Mensaje = "Formato inválido en un campo (verifique IP/IP remota)." };
                    }

                    if (pg.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        var c = (pg.ConstraintName ?? "").ToLowerInvariant();

                        if (c.Contains("ux_srv_hostname_ci"))
                            return new RespuestaComunDTO { Ok = false, Mensaje = "Hostname ya existe (se compara sin mayúsculas/espacios)." };

                        if (c.Contains("ux_srv_ip_so"))
                            return new RespuestaComunDTO { Ok = false, Mensaje = "La IP del SO ya existe en otro servidor." };

                        if (c.Contains("ux_srv_ip_remota"))
                            return new RespuestaComunDTO { Ok = false, Mensaje = "La IP remota ya existe en otro servidor." };

                        if (c.Contains("ux_srv_mac"))
                            return new RespuestaComunDTO { Ok = false, Mensaje = "La dirección MAC ya existe en otro servidor." };

                        return new RespuestaComunDTO { Ok = false, Mensaje = "Datos duplicados (hostname/IP/MAC)." };
                    }

                    if (pg.SqlState == PostgresErrorCodes.ForeignKeyViolation)
                    {
                        return new RespuestaComunDTO { Ok = false, Mensaje = "Referencia inválida (tipo de servidor o SO no existe)." };
                    }
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
            while (ex != null)
            {
                if (ex is PostgresException p) return p;
                ex = ex.InnerException;
            }
            return null;
        }

    }
}
