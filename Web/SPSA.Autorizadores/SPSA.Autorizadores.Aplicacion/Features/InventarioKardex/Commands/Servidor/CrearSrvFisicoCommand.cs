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
    public class CrearSrvFisicoCommand : IRequest<RespuestaComunDTO>
    {
        public long SerieProductoId { get; set; }   // requerido
        public short TipoId { get; set; }           // requerido
        public string Hostname { get; set; }        // requerido
        public string IpSo { get; set; }              // opcional (INET)
        public decimal? RamGb { get; set; }
        public int? CpuSockets { get; set; }
        public int? CpuCores { get; set; }
        public string HddTotal { get; set; }
        public long? SoId { get; set; }
        public string MacAddress { get; set; }
        public string ConexionRemota { get; set; }
        public string IpRemota { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class CrearSrvFisicoHandler : IRequestHandler<CrearSrvFisicoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public CrearSrvFisicoHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearSrvFisicoCommand command, CancellationToken ct)
        {
            try
            {
                // Validaciones de entrada
                if (command.SerieProductoId <= 0) throw new InvalidOperationException("Serie de producto no válida.");
                if (string.IsNullOrWhiteSpace(command.Hostname)) throw new InvalidOperationException("Hostname es obligatorio.");
                if (command.TipoId <= 0) throw new InvalidOperationException("Tipo de servidor es obligatorio.");

                // Normalizaciones
                var hostnameNorm = command.Hostname.Trim();
                var ipSo = string.IsNullOrWhiteSpace(command.IpSo) ? null : command.IpSo.Trim();
                var ipRemota = string.IsNullOrWhiteSpace(command.IpRemota) ? null : command.IpRemota.Trim();
                var mac = string.IsNullOrWhiteSpace(command.MacAddress) ? null : command.MacAddress.Trim();


                // Serie + ubicación
                var serie = await _ctx.RepositorioMaeSerieProducto
                    .Obtener(s => s.Id == command.SerieProductoId)
                    .Include(s => s.Producto) // si tienes navegación a mae_producto
                    .FirstOrDefaultAsync(ct);

                if (serie == null) throw new InvalidOperationException("No se encontró la serie indicada.");
                if (string.Equals((serie.IndEstado ?? "").ToUpper(), "DE_BAJA"))
                    throw new InvalidOperationException("La serie está de baja.");

                // Evitar duplicar registro de servidor para la misma serie
                var yaExiste = await _ctx.RepositorioSrvSerieCaracteristica
                    .Obtener(x => x.SerieProductoId == command.SerieProductoId)
                    .AnyAsync(ct);
                if (yaExiste) throw new InvalidOperationException("Ya existe un detalle de servidor para esta serie.");

                // -------- (Opcional) validación básica de IPs (dejamos que Postgres inet sea la autoridad final)
                // Nota: inet permite CIDR (e.g., 10.0.0.0/24); no usamos IPAddress.TryParse para no rechazar CIDR válidos.

                var now = DateTime.Now;

                var srv = new Dominio.Entidades.SrvFisico
                {
                    SerieProductoId = command.SerieProductoId,
                    TipoId = command.TipoId,
                    Hostname = hostnameNorm,
                    IpSo = ipSo,
                    RamGb = command.RamGb,
                    CpuSockets = command.CpuSockets,
                    CpuCores = command.CpuCores,
                    HddTotal = command.HddTotal,
                    SoId = command.SoId,
                    MacAddress = mac,
                    ConexionRemota = command.ConexionRemota?.Trim(),
                    IpRemota = ipRemota,
                    UsuCreacion = string.IsNullOrWhiteSpace(command.UsuCreacion) ? "system" : command.UsuCreacion.Trim(),
                    FecCreacion = now
                };

                _ctx.RepositorioSrvSerieCaracteristica.Agregar(srv);
                await _ctx.GuardarCambiosAsync();

                return new RespuestaComunDTO { Ok = true, Mensaje = "Servidor creado correctamente." };
            }
            catch (DbUpdateException dbEx)
            {
                var pg = FindPg(dbEx);
                if (pg != null)
                {
                    // 22P02: invalid_text_representation (p. ej., inet inválido)
                    if (pg.SqlState == "22P02")
                    {
                        return new RespuestaComunDTO { Ok = false, Mensaje = "Formato inválido en un campo (verifique MAC/IP so/IP remota)." };
                    }

                    if (pg.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        // ConstraintName viene en minúsculas si el índice no está entrecomillado
                        var c = (pg.ConstraintName ?? "").ToLowerInvariant();

                        if (c.Contains("ux_srv_hostname_ci"))
                            return new RespuestaComunDTO { Ok = false, Mensaje = "Hostname ya existe (se compara sin mayúsculas/espacios)." };

                        if (c.Contains("ux_srv_ip_so"))
                            return new RespuestaComunDTO { Ok = false, Mensaje = "La IP del SO ya existe en otro servidor." };

                        if (c.Contains("ux_srv_ip_remota"))
                            return new RespuestaComunDTO { Ok = false, Mensaje = "La IP remota ya existe en otro servidor." };

                        if (c.Contains("ux_srv_mac"))
                            return new RespuestaComunDTO { Ok = false, Mensaje = "La dirección MAC ya existe en otro servidor." };

                        // Fallback
                        return new RespuestaComunDTO { Ok = false, Mensaje = "Datos duplicados (hostname/IP/MAC)." };
                    }

                    if (pg.SqlState == PostgresErrorCodes.ForeignKeyViolation)
                    {
                        return new RespuestaComunDTO { Ok = false, Mensaje = "Referencia inválida (tipo de servidor o SO no existe)." };
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
            while (ex != null)
            {
                if (ex is PostgresException p) return p;
                ex = ex.InnerException;
            }
            return null;
        }

    }
}
