using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Npgsql;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands
{
    public class ActualizarMaeColaboradorExtCommand : IRequest<RespuestaComunDTO>
    {
        public string CodLocalAlterno { get; set; }
        public string CodigoOfisis { get; set; }
        public DateTime FechaIngresoEmpresa { get; set; }
        public DateTime? FechaCeseTrabajador { get; set; }
        public string TiSitu { get; set; }
        public string PuestoTrabajo { get; set; }
        public string MotiSepa { get; set; }
        public string UsuModifica { get; set; }
        public DateTime FecModifica { get; set; } = DateTime.UtcNow;

        public string CodLocalAlternoAnterior { get; set; }
    }

    public class ActualizarMaeColaboradorExtHandler : IRequestHandler<ActualizarMaeColaboradorExtCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeColaboradorExtHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = Logger.SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeColaboradorExtCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                // 1. Obtener el registro actual para comparar (similar a lo mostrado anteriormente)
                var selectSql = @"
            SELECT 
                ""FE_INGR_EMPR"", 
                ""FE_CESE_TRAB"", 
                ""TI_SITU"", 
                ""NO_PUES_TRAB"", 
                ""NO_MOTI_SEPA""
            FROM ""SGP"".""MAE_COLABORADOR_EXT""
            WHERE ""COD_LOCAL_ALTERNO"" = @codLocalAlternoAnterior
              AND ""CODIGO_OFISIS"" = @codigoOfisis;";

                var selectParams = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@codLocalAlternoAnterior", Convert.ToInt32(request.CodLocalAlternoAnterior)),
            new NpgsqlParameter("@codigoOfisis", request.CodigoOfisis)
        };

                var oldRecord = await _contexto.RepositorioMaeColaboradorExt.Obtener(x => x.CodigoOfisis == request.CodigoOfisis).FirstOrDefaultAsync();
                if (oldRecord == null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontró el registro a actualizar.";
                    return respuesta;
                }

                // Preparar auditorías (sin transacción, sólo almacenar en una lista)
                var auditCommands = new List<(string sql, NpgsqlParameter[] parameters)>();

                void AgregarAuditoria(string campo, string valorAnterior, string valorNuevo)
                {
                    if (valorAnterior != valorNuevo)
                    {
                        var auditSql = @"
                    INSERT INTO ""SGP"".""HIS_COLAB_MODIFICACION""
                        (""CODIGO_OFISIS"", ""CAMPO_MODIFICADO"", ""VALOR_ANTERIOR"", ""VALOR_NUEVO"", ""USU_MODIFICA"", ""FEC_MODIFICACION"")
                    VALUES (@codigoOfisis, @campo, @valorAnterior, @valorNuevo, @usuModifica, CURRENT_TIMESTAMP);";
                        var auditParams = new NpgsqlParameter[]
                        {
                    new NpgsqlParameter("@codigoOfisis", request.CodigoOfisis),
                    new NpgsqlParameter("@campo", campo),
                    new NpgsqlParameter("@valorAnterior", valorAnterior ?? string.Empty),
                    new NpgsqlParameter("@valorNuevo", valorNuevo ?? string.Empty),
                    new NpgsqlParameter("@usuModifica", request.UsuModifica ?? string.Empty)
                        };
                        auditCommands.Add((auditSql, auditParams));
                    }
                }

                AgregarAuditoria("FE_INGR_EMPR", oldRecord.FechaIngresoEmpresa.ToString("yyyy-MM-dd"), request.FechaIngresoEmpresa.ToString("yyyy-MM-dd"));
                AgregarAuditoria("FE_CESE_TRAB", oldRecord.FechaCeseTrabajador?.ToString("yyyy-MM-dd"), request.FechaCeseTrabajador?.ToString("yyyy-MM-dd"));
                AgregarAuditoria("TI_SITU", oldRecord.TiSitu, request.TiSitu);
                AgregarAuditoria("NO_PUES_TRAB", oldRecord.PuestoTrabajo, request.PuestoTrabajo);
                AgregarAuditoria("NO_MOTI_SEPA", oldRecord.MotiSepa, request.MotiSepa);

                // 2. Ejecutar la actualización
                var updateSql = @"
            UPDATE ""SGP"".""MAE_COLABORADOR_EXT""
            SET 
                ""COD_LOCAL_ALTERNO"" = @nuevoCodLocalAlterno,
                ""FE_INGR_EMPR"" = @fechaIngreso,
                ""FE_CESE_TRAB"" = @fechaCese,
                ""TI_SITU"" = @tiSitu,
                ""NO_PUES_TRAB"" = @puestoTrabajo,
                ""NO_MOTI_SEPA"" = @motiSepa,
                ""USU_MODIFICA"" = @usuModifica,
                ""FEC_MODIFICA"" = @fecModifica
            WHERE 
                ""COD_LOCAL_ALTERNO"" = @codLocalAlternoAnterior
                AND ""CODIGO_OFISIS"" = @codigoOfisis;";

                var updateParams = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@nuevoCodLocalAlterno", Convert.ToInt32(request.CodLocalAlterno)),
            new NpgsqlParameter("@fechaIngreso", request.FechaIngresoEmpresa),
            new NpgsqlParameter("@fechaCese", request.FechaCeseTrabajador),
            new NpgsqlParameter("@tiSitu", request.TiSitu),
            new NpgsqlParameter("@puestoTrabajo", request.PuestoTrabajo),
            new NpgsqlParameter("@motiSepa", request.MotiSepa ?? string.Empty),
            new NpgsqlParameter("@usuModifica", request.UsuModifica ?? string.Empty),
            new NpgsqlParameter("@fecModifica", request.FecModifica),
            new NpgsqlParameter("@codigoOfisis", request.CodigoOfisis),
            new NpgsqlParameter("@codLocalAlternoAnterior", Convert.ToInt32(request.CodLocalAlternoAnterior))
        };

                var affectedRows = await _contexto.RepositorioMaeColaboradorExt.ExecuteSqlCommandAsync(updateSql, updateParams.ToArray());

                // 3. Si la actualización fue exitosa, registrar auditorías
                if (affectedRows > 0)
                {
                    foreach (var (auditSql, auditParams) in auditCommands)
                    {
                        try
                        {
                            await _contexto.RepositorioMaeColaboradorExt.ExecuteSqlCommandAsync(auditSql, auditParams);
                        }
                        catch (Exception auditEx)
                        {
                            // Aquí puedes loguear el error de auditoría pero no evitar que la actualización principal ya se haya realizado.
                            _logger.Error(auditEx, "Error al insertar registro de auditoría para CODIGO_OFISIS {codigo}", request.CodigoOfisis);
                        }
                    }
                    respuesta.Mensaje = "Colaborador externo actualizado exitosamente.";
                }
                else
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontraron registros para actualizar.";
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, "Error al actualizar colaborador externo: " + ex.Message);
            }

            return respuesta;
        }



        //public async Task<RespuestaComunDTO> Handle(ActualizarMaeColaboradorExtCommand request, CancellationToken cancellationToken)
        //{
        //    var respuesta = new RespuestaComunDTO { Ok = true };

        //    //if (request.FechaCeseTrabajador == DateTime.MinValue)
        //    //{
        //    //    request.FechaCeseTrabajador = null;
        //    //}

        //    try
        //    {
        //        var sql = @"update
        //                     ""SGP"".""MAE_COLABORADOR_EXT""
        //                    set
        //                     ""COD_LOCAL_ALTERNO"" = @nuevoCodLocalAnterno,
        //                     ""FE_INGR_EMPR"" = @fechaIngreso,
        //                     ""FE_CESE_TRAB"" = @fechaCese,
        //                     ""TI_SITU"" = @tiSitu,
        //                     ""NO_PUES_TRAB"" = @puestoTrabajo,
        //                     ""NO_MOTI_SEPA"" = @motiSepa,
        //                     ""USU_MODIFICA"" = @usuModifica,
        //                     ""FEC_MODIFICA"" = @fecModifica
        //                    where
        //                     ""COD_LOCAL_ALTERNO"" = @codLocalAlternoAnterior
        //                     and ""CODIGO_OFISIS"" = @codigoOfisis;";

        //        var parameters = new List<NpgsqlParameter>
        //{
        //    new NpgsqlParameter("@nuevoCodLocalAnterno", Convert.ToInt32(request.CodLocalAlterno)),
        //    new NpgsqlParameter("@fechaIngreso", request.FechaIngresoEmpresa),
        //    new NpgsqlParameter("@fechaCese", request.FechaCeseTrabajador),
        //    new NpgsqlParameter("@tiSitu", request.TiSitu),
        //    new NpgsqlParameter("@puestoTrabajo", request.PuestoTrabajo),
        //    new NpgsqlParameter("@motiSepa", request.MotiSepa ?? string.Empty),
        //    new NpgsqlParameter("@usuModifica", request.UsuModifica ?? string.Empty),
        //    new NpgsqlParameter("@fecModifica", request.FecModifica),
        //    new NpgsqlParameter("@codigoOfisis", request.CodigoOfisis),
        //    new NpgsqlParameter("@codLocalAlternoAnterior", Convert.ToInt32(request.CodLocalAlternoAnterior))
        //};

        //        var affectedRows = await _contexto.RepositorioMaeColaboradorExt.ExecuteSqlCommandAsync(sql, parameters.ToArray());

        //        if (affectedRows > 0)
        //        {
        //            respuesta.Mensaje = "Colaborador externo actualizado exitosamente.";
        //        }
        //        else
        //        {
        //            respuesta.Ok = false;
        //            respuesta.Mensaje = "No se encontraron registros para actualizar.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Ok = false;
        //        respuesta.Mensaje = ex.Message;
        //        _logger.Error(ex, "Error al actualizar colaborador externo: " + ex.Message);
        //    }

        //    return respuesta;

        //}
    }
}
