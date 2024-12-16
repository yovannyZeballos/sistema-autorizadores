using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.Horarios.Commands
{
    public class EliminarMaeHorarioCommand : IRequest<RespuestaComunDTO>
    {
        //public string CodEmpresa { get; set; } = string.Empty; 
        //public string CodCadena { get; set; } = string.Empty;
        //public string CodRegion { get; set; } = string.Empty;
        //public string CodZona { get; set; } = string.Empty;
        //public string CodLocal { get; set; } = string.Empty;
        //public int NumDia { get; set; } = 0;

        public List<MaeHorarioDTO> Horarios { get; set; }
    }

    public class EliminarMaeHorarioHandler : IRequestHandler<EliminarMaeHorarioCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaeHorarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeHorarioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                foreach (var horarioEliminar in request.Horarios)
                {
                    var horario = await _contexto.RepositorioMaeHorario.Obtener(x => x.CodEmpresa == horarioEliminar.CodEmpresa && x.CodCadena == horarioEliminar.CodCadena && x.CodRegion == horarioEliminar.CodRegion
                                                                    && x.CodZona == horarioEliminar.CodZona && x.CodLocal == horarioEliminar.CodLocal && x.NumDia == horarioEliminar.NumDia).FirstOrDefaultAsync();
                    if (horario is null)
                    {
                        respuesta.Ok = false;
                        respuesta.Mensaje = "Horario no existe";
                        return respuesta;
                    }
                    _contexto.RepositorioMaeHorario.Eliminar(horario);
                    await _contexto.GuardarCambiosAsync();
                }

                respuesta.Mensaje = "Horario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, respuesta.Mensaje);
            }
            return respuesta;
        }
    }
}
