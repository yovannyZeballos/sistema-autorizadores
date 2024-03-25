
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

namespace SPSA.Autorizadores.Aplicacion.Features.Aperturas.Queries
{
    public class ListarAperturaQuery : IRequest<GenericResponseDTO<List<ListarAperturaDTO>>>
    {
    }

    public class ListarAperturaHandler : IRequestHandler<ListarAperturaQuery, GenericResponseDTO<List<ListarAperturaDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IBCTContexto _contexto;
        private readonly ILogger _logger;

        public ListarAperturaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarAperturaDTO>>> Handle(ListarAperturaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarAperturaDTO>> { Ok = true, Data = new List<ListarAperturaDTO>() };

            try
            {
                var aperturas = await _contexto.RepositorioApertura.Obtener()
                    .OrderBy(x => x.CodLocalPMM)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarAperturaDTO>>(aperturas);
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
