using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.Horarios.Queries
{
    public class ObtenerMaeHorarioQuery : IRequest<GenericResponseDTO<ObtenerMaeHorarioDTO>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public int NumDia { get; set; }
    }

    public class ObtenerMaeHorarioHandler : IRequestHandler<ObtenerMaeHorarioQuery, GenericResponseDTO<ObtenerMaeHorarioDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerMaeHorarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerMaeHorarioDTO>> Handle(ObtenerMaeHorarioQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerMaeHorarioDTO> { Ok = true };
            try
            {
                var horario = await _contexto.RepositorioMaeHorario.Obtener(s => s.CodEmpresa == request.CodEmpresa && s.CodCadena == request.CodCadena && s.CodRegion == request.CodRegion && s.CodZona == request.CodZona && s.CodLocal == request.CodLocal && s.NumDia == request.NumDia).FirstOrDefaultAsync();
                if (horario is null)
                {
                    response.Ok = false;
                    response.Mensaje = "Horario no existe";
                    return response;
                }

                var horarioDto = _mapper.Map<ObtenerMaeHorarioDTO>(horario);
                response.Data = horarioDto;
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
                _logger.Error(ex, response.Mensaje);
            }
            return response;
        }
    }
}
