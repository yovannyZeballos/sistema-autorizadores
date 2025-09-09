using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands
{
    public class CrearMaeColaboradorExtCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodigoOfisis { get; set; }
        public string ApelPaterno { get; set; }
        public string ApelMaterno { get; set; }
        public string NombreTrabajador { get; set; }
        public string TipoDocIdent { get; set; }
        public string NumDocIndent { get; set; }
        public DateTime? FechaIngresoEmpresa { get; set; }
        public DateTime? FechaCeseTrabajador { get; set; }
        public string IndActivo { get; set; }
        public string PuestoTrabajo { get; set; }
        public string MotiSepa { get; set; }
        public string IndPersonal { get; set; }
        public string TipoUsuario { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; } = DateTime.UtcNow;
    }

    public class CrearMaeColaboradorExtHandler : IRequestHandler<CrearMaeColaboradorExtCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeColaboradorExtHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeColaboradorExtCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                // --- Normalización mínima ---
                request.IndPersonal = (request.IndPersonal ?? "").Trim().ToUpperInvariant();
                request.TipoDocIdent = (request.TipoDocIdent ?? "").Trim().ToUpperInvariant();
                request.IndActivo = (request.IndActivo ?? "").Trim().ToUpperInvariant();
                request.NumDocIndent = (request.NumDocIndent ?? "").Trim();


                // --- Validaciones mínimas existentes ---
                if (string.IsNullOrWhiteSpace(request.CodEmpresa))
                    throw new InvalidOperationException("Debe indicar la empresa.");
                if (string.IsNullOrWhiteSpace(request.CodLocal))
                    throw new InvalidOperationException("Debe indicar el local.");
                if (string.IsNullOrWhiteSpace(request.TipoDocIdent))
                    throw new InvalidOperationException("Debe indicar el tipo de documento.");
                if (string.IsNullOrWhiteSpace(request.NumDocIndent))
                    throw new InvalidOperationException("Debe indicar el número de documento.");
                if (!request.FechaIngresoEmpresa.HasValue)
                    throw new InvalidOperationException("Debe indicar la fecha de ingreso.");

                if (request.IndPersonal == "S")
                {
                    if (string.Equals(request.NumDocIndent, "00000000", StringComparison.Ordinal))
                        throw new InvalidOperationException("Para personal 'S', el número de documento no puede ser 00000000.");

                    var existeActivoMismoDoc = await _contexto.RepositorioMaeColaboradorExt
                        .Obtener(x => x.IndPersonal == "S"
                                   && x.NumDocIndent == request.NumDocIndent
                                   && x.IndActivo == "S")
                        .AnyAsync(cancellationToken);

                    if (existeActivoMismoDoc)
                        throw new InvalidOperationException("Ya existe un colaborador personal ACTIVO con el mismo número de documento.");
                }


                var colabExt = new Mae_ColaboradorExt {
                    CodEmpresa = request.CodEmpresa,
                    CodLocal = request.CodLocal,
                    CodigoOfisis = request.CodigoOfisis,
                    ApelPaterno = request.ApelPaterno,
                    ApelMaterno = request.ApelMaterno,
                    NombreTrabajador = request.NombreTrabajador,
                    TipoDocIdent = request.TipoDocIdent,
                    NumDocIndent = request.NumDocIndent,
                    FechaIngresoEmpresa = request.FechaIngresoEmpresa.Value,
                    FechaCeseTrabajador = request.FechaCeseTrabajador,
                    //IndActivo = request.IndActivo,
                    IndActivo = "S",
                    PuestoTrabajo = request.PuestoTrabajo,
                    MotiSepa = request.MotiSepa,
                    IndPersonal = request.IndPersonal,
                    TipoUsuario = request.TipoUsuario,
                    UsuCreacion = request.UsuCreacion,
                    FecCreacion = request.FecCreacion
                };

                _contexto.RepositorioMaeColaboradorExt.Agregar(colabExt);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Colaborador externo creado exitosamente.";
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

                    string msg = null;

                    switch (pg.SqlState)
                    {
                        case PostgresErrorCodes.UniqueViolation: // 23505
                            // Mapea por nombre de constraint si aplica
                            if (!string.IsNullOrEmpty(pg.ConstraintName))
                            {
                                if (pg.ConstraintName.Equals("pk_mae_colaborador_ext", StringComparison.OrdinalIgnoreCase))
                                    msg = "Ya existe un colaborador con el mismo (Empresa, Local, Ofisis).";

                                // antiguo índice
                                if (pg.ConstraintName.Equals("ux_mae_colab_ext_numdoc", StringComparison.OrdinalIgnoreCase))
                                    msg = "Ya existe un colaborador personal con ese documento.";

                                // nuevo índice (solo activos)
                                if (pg.ConstraintName.Equals("ux_mae_colab_ext_numdoc_act", StringComparison.OrdinalIgnoreCase))
                                    msg = "Ya existe un colaborador personal ACTIVO con ese documento.";
                            }
                            if (msg == null) msg = "No se pudo guardar: datos duplicados (restricción de unicidad).";

                            respuesta.Mensaje = msg;

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
