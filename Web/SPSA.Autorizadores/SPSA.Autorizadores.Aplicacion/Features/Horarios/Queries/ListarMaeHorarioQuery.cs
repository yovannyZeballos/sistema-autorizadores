using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Linq;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Horarios.Queries
{
    public class ListarMaeHorarioQuery : IRequest<GenericResponseDTO<List<ListarMaeHorarioDTO>>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
    }

    public class ListarMaeHorarioHandler : IRequestHandler<ListarMaeHorarioQuery, GenericResponseDTO<List<ListarMaeHorarioDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarMaeHorarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeHorarioDTO>>> Handle(ListarMaeHorarioQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeHorarioDTO>> { Ok = true, Data = new List<ListarMaeHorarioDTO>() };
            try
            {
                var horarios = await _contexto.RepositorioMaeHorario.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal)
                    .OrderBy(x => x.NumDia)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeHorarioDTO>>(horarios);
                response.Ok = true;
                response.Mensaje = "Se ha generado la lista correctamente";
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
