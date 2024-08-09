using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Linq;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Colaborador.Queries
{
    public class ListarMaeColaboradorQuery : IRequest<GenericResponseDTO<List<ListarMaeColaboradorDTO>>>
    {
    }

    public class ListarMaeColaboradorHandler : IRequestHandler<ListarMaeColaboradorQuery, GenericResponseDTO<List<ListarMaeColaboradorDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeColaboradorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeColaboradorDTO>>> Handle(ListarMaeColaboradorQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeColaboradorDTO>> { Ok = true, Data = new List<ListarMaeColaboradorDTO>() };

            try
            {
                var colaboradores = await _contexto.RepositorioMaeColaborador.Obtener()
                    .OrderBy(x => x.NoApelPate)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeColaboradorDTO>>(colaboradores);
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
