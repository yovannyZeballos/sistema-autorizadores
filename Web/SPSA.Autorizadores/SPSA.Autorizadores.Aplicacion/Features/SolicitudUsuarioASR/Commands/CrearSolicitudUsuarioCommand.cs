using System;
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

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands
{
    public class CrearSolicitudUsuarioCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodColaborador { get; set; }
        public string TipUsuario { get; set; }
        public string TipColaborador { get; set; }
        public string UsuSolicita { get; set; }
        public DateTime FecSolicita { get; set; } = DateTime.UtcNow;
        public string TipAccion { get; set; }
        public string IndAprobado { get; set; } = "N";
        public string Motivo { get; set; }
    }

    public class CrearSolicitudUsuarioHandler : IRequestHandler<CrearSolicitudUsuarioCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public CrearSolicitudUsuarioHandler()
        {
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearSolicitudUsuarioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                // --- Normalización mínima ---
                request.CodEmpresa = (request.CodEmpresa ?? "").Trim().ToUpperInvariant();
                request.CodLocal = (request.CodLocal ?? "").Trim().ToUpperInvariant();
                request.CodColaborador = (request.CodColaborador ?? "").Trim().ToUpperInvariant();
                request.UsuSolicita = string.IsNullOrWhiteSpace(request.UsuSolicita)
                                        ? Environment.UserName ?? "system"
                                        : request.UsuSolicita.Trim();
                var indAprobado = "N"; // creación siempre en 'N'

                // --- Validaciones mínimas ---
                if (string.IsNullOrWhiteSpace(request.CodEmpresa))
                    throw new InvalidOperationException("Debe indicar la empresa.");
                if (string.IsNullOrWhiteSpace(request.CodLocal))
                    throw new InvalidOperationException("Debe indicar el local.");
                if (string.IsNullOrWhiteSpace(request.CodColaborador))
                    throw new InvalidOperationException("Debe indicar el código de colaborador.");

                if (!IsIn(request.TipUsuario, "A", "C") && !string.IsNullOrEmpty(request.TipUsuario))
                    throw new InvalidOperationException("Tipo de usuario inválido (use 'A' o 'C').");

                if (!IsIn(request.TipColaborador, "E", "I") && !string.IsNullOrEmpty(request.TipColaborador))
                    throw new InvalidOperationException("Tipo de colaborador inválido (use 'E' o 'I').");

                if (!IsIn(request.TipAccion, "CREAR", "ELIMINAR"))
                    throw new InvalidOperationException("Tipo de acción inválido (use 'CREAR' o 'ELIMINAR').");


                // --- Regla de “solicitud abierta” (pendiente/no eliminada) por colaborador/local/acción ---
                var existeAbierta = await _contexto.RepositorioSolicitudUsuarioASR.Obtener(x =>
                        x.CodEmpresa == request.CodEmpresa &&
                        x.CodLocal == request.CodLocal &&
                        x.CodColaborador == request.CodColaborador &&
                        x.TipAccion == request.TipAccion &&
                        x.FecElimina == null &&
                        x.IndAprobado != "S")
                    .AnyAsync(cancellationToken);

                if (existeAbierta)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = $"Ya existe una solicitud pendiente para el colaborador {request.CodColaborador} en {request.CodEmpresa}-{request.CodLocal} para la acción '{request.TipAccion}'.";
                    return respuesta;
                }


                //bool existe = await _contexto.RepositorioSolicitudUsuarioASR.Existe(x => x.CodColaborador == request.CodColaborador && x.IndAprobado == "S");
                //if (existe)
                //{
                //    respuesta.Ok = false;
                //    respuesta.Mensaje = $"Ya existe una solicitud abierta para colaborador {request.CodColaborador}.";
                //    return respuesta;
                //}

                // --- Crear entidad ---
                var solicitud = new ASR_SolicitudUsuario
                {
                    CodEmpresa = request.CodEmpresa,
                    CodLocal = request.CodLocal,
                    CodColaborador = request.CodColaborador,
                    TipUsuario = string.IsNullOrEmpty(request.TipUsuario) ? null : request.TipUsuario,
                    TipColaborador = string.IsNullOrEmpty(request.TipColaborador) ? null : request.TipColaborador,
                    UsuSolicita = request.UsuSolicita,
                    FecSolicita = request.FecSolicita,
                    TipAccion = request.TipAccion,
                    IndAprobado = indAprobado,    // 'N' en alta
                    Motivo = string.IsNullOrWhiteSpace(request.Motivo) ? null : request.Motivo.Trim()
                };

                _contexto.RepositorioSolicitudUsuarioASR.Agregar(solicitud);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Solicitud creado exitosamente.";

            }
            catch (DbUpdateException dbEx)
            {
                var pg = FindPostgresException(dbEx);

                if (pg != null)
                {
                    _logger.Error(dbEx,
                        "Error PG: SqlState={SqlState}, Constraint={Constraint}, Detail={Detail}",
                        pg.SqlState, pg.ConstraintName, pg.Detail);

                    string msg = null;

                    switch (pg.SqlState)
                    {
                        case PostgresErrorCodes.UniqueViolation: // 23505
                            // Mapea por nombre de constraint/índice si lo creaste (ej. ux_asr_solicitud_activa)
                            if (!string.IsNullOrEmpty(pg.ConstraintName))
                            {
                                if (pg.ConstraintName.Equals("ux_asr_solicitud_activa", StringComparison.OrdinalIgnoreCase))
                                    msg = "Ya existe una solicitud pendiente con los mismos datos.";
                            }
                            if (msg == null) msg = "No se pudo guardar: datos duplicados (restricción de unicidad).";
                            break;

                        case PostgresErrorCodes.ForeignKeyViolation: // 23503
                            // Puedes inspeccionar pg.ConstraintName: fk_asr_su_empresa / fk_asr_su_local
                            msg = "No se pudo guardar: datos relacionados no válidos (empresa/local/colaborador).";
                            break;

                        case PostgresErrorCodes.CheckViolation: // 23514
                            msg = "No se pudo guardar: algún valor no cumple las reglas de validación.";
                            break;

                        default:
                            msg = $"Error de base de datos ({pg.SqlState}).";
                            break;
                    }

                    return new RespuestaComunDTO { Ok = false, Mensaje = msg };
                }

                _logger.Error(dbEx, "Error al guardar cambios (no-PG)");
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al guardar los datos.";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, respuesta.Mensaje);
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }

        static bool IsIn(string v, params string[] options)
           => !string.IsNullOrEmpty(v) && options.Any(o => string.Equals(v, o, StringComparison.Ordinal));


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
