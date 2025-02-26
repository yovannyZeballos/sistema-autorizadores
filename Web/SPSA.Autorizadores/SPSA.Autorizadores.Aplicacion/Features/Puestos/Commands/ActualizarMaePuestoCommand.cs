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

namespace SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands
{
    public class ActualizarMaePuestoCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodPuesto { get; set; }
        //public string DesPuesto { get; set; }
        public string IndAutAut { get; set; }
        public string IndAutOpe { get; set; }
        public string IndManAut { get; set; }
        public string IndManOpe { get; set; }
        public DateTime? FecAsigna { get; set; }
        public string UsuAsigna { get; set; }
        public DateTime? FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecElimina { get; set; }
        public string UsuElimina { get; set; }
    }

    public class ActualizarMaePuestoHandler : IRequestHandler<ActualizarMaePuestoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaePuestoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = Logger.SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaePuestoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var oldRecord = await _contexto.RepositorioMaePuesto.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodPuesto == request.CodPuesto).FirstOrDefaultAsync();
                if (oldRecord == null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontró el registro a actualizar.";
                    return respuesta;
                }

                var auditCommands = new List<(string sql, NpgsqlParameter[] parameters)>();

                void AgregarAuditoria(string campo, string valorAnterior, string valorNuevo)
                {
                    if (valorAnterior != valorNuevo)
                    {
                        var auditSql = @"
                        INSERT INTO ""SGP"".""MAE_PUESTO_HIST""
                            (""CO_EMPR"", ""CO_PUES_TRAB"", ""CAMPO_MODIFICADO"", ""VALOR_ANTERIOR"", ""VALOR_NUEVO"", ""USU_MODIFICA"", ""FEC_MODIFICA"")
                        VALUES (@codEmpresa, @codPuesto, @campo, @valorAnterior, @valorNuevo, @usuModifica, CURRENT_TIMESTAMP);";

                        var auditParams = new NpgsqlParameter[]
                        {
                            new NpgsqlParameter("@codEmpresa", request.CodEmpresa),
                            new NpgsqlParameter("@codPuesto", request.CodPuesto),
                            new NpgsqlParameter("@campo", campo),
                            new NpgsqlParameter("@valorAnterior", valorAnterior ?? string.Empty),
                            new NpgsqlParameter("@valorNuevo", valorNuevo ?? string.Empty),
                            new NpgsqlParameter("@usuModifica", request.UsuAsigna ?? request.UsuElimina)
                        };

                        auditCommands.Add((auditSql, auditParams));
                    }
                }

                // Actualizar la descripción del puesto si cambia
                //if (oldRecord.DesPuesto != request.DesPuesto)
                //{
                //    AgregarAuditoria("DE_PUES_TRAB", oldRecord.DesPuesto, request.DesPuesto);
                //    oldRecord.DesPuesto = request.DesPuesto;
                //}

                // IND_AUT_AUT
                if (oldRecord.IndAutAut != request.IndAutAut)
                {
                    AgregarAuditoria("IND_AUT_AUT", oldRecord.IndAutAut, request.IndAutAut);
                    oldRecord.IndAutAut = request.IndAutAut;

                    if (request.IndAutAut == "S")
                    {
                        oldRecord.UsuAsigna = request.UsuAsigna ?? "";
                        oldRecord.FecAsigna = DateTime.Now;
                    }
                    else if (request.IndAutAut == "N")
                    {
                        oldRecord.UsuElimina = request.UsuElimina ?? "";
                        oldRecord.FecElimina = DateTime.Now;
                    }
                }

                // IND_AUT_OPE
                if (oldRecord.IndAutOpe != request.IndAutOpe)
                {
                    AgregarAuditoria("IND_AUT_OPE", oldRecord.IndAutOpe, request.IndAutOpe);
                    oldRecord.IndAutOpe = request.IndAutOpe;
                    if (request.IndAutOpe == "S")
                    {
                        oldRecord.UsuAsigna = request.UsuAsigna ?? "";
                        oldRecord.FecAsigna = DateTime.Now;
                    }
                    else if (request.IndAutOpe == "N")
                    {
                        oldRecord.UsuElimina = request.UsuElimina ?? "";
                        oldRecord.FecElimina = DateTime.Now;
                    }
                }

                // IND_MAN_AUT
                if (oldRecord.IndManAut != request.IndManAut)
                {
                    AgregarAuditoria("IND_MAN_AUT", oldRecord.IndManAut, request.IndManAut);
                    oldRecord.IndManAut = request.IndManAut;
                    if (request.IndManAut == "S")
                    {
                        oldRecord.UsuAsigna = request.UsuAsigna ?? "";
                        oldRecord.FecAsigna = DateTime.Now;
                    }
                    else if (request.IndManAut == "N")
                    {
                        oldRecord.UsuElimina = request.UsuElimina ?? "";
                        oldRecord.FecElimina = DateTime.Now;
                    }
                }

                // IND_MAN_OPE
                if (oldRecord.IndManOpe != request.IndManOpe)
                {
                    AgregarAuditoria("IND_MAN_OPE", oldRecord.IndManOpe, request.IndManOpe);
                    oldRecord.IndManOpe = request.IndManOpe;
                    if (request.IndManOpe == "S")
                    {
                        oldRecord.UsuAsigna = request.UsuAsigna ?? "";
                        oldRecord.FecAsigna = DateTime.Now;
                    }
                    else if (request.IndManOpe == "N")
                    {
                        oldRecord.UsuElimina = request.UsuElimina ?? "";
                        oldRecord.FecElimina = DateTime.Now;
                    }
                }

                // Actualizamos el registro principal
                await _contexto.GuardarCambiosAsync();

                // Ejecutar los comandos de auditoría (se recomienda que esto se haga en una transacción, si es posible)
                foreach (var (auditSql, auditParams) in auditCommands)
                {
                    try
                    {
                        await _contexto.RepositorioMaeColaboradorExt.ExecuteSqlCommandAsync(auditSql, auditParams);
                    }
                    catch (Exception auditEx)
                    {
                        _logger.Error(auditEx, "Error al registrar auditoría para {0} - {1}", request.CodEmpresa, request.CodPuesto);
                    }
                }

                respuesta.Mensaje = "Puesto actualizado exitosamente.";

            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, "Error al actualizar colaborador externo: " + ex.Message);
            }

            return respuesta;
        }
    }
}
