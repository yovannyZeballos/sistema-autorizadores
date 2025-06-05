using System.Collections.Generic;
using AutoMapper;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Linq;
using System.Data.Entity;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.OperadorMdr;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.OperadorMdr
{
    public class ListarMdrOperadorQuery : IRequest<GenericResponseDTO<List<ListarMdrOperadorDto>>>
    {

    }

    public class ListarMdrOperadorHandler : IRequestHandler<ListarMdrOperadorQuery, GenericResponseDTO<List<ListarMdrOperadorDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMdrOperadorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMdrOperadorDto>>> Handle(ListarMdrOperadorQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMdrOperadorDto>>
            {
                Ok = true,
                Data = new List<ListarMdrOperadorDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMdrOperador
                    .Obtener()
                    .OrderBy(x => x.NomOperador)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarMdrOperadorDto>>(listaEntidades);
                response.Ok = true;
                response.Mensaje = "Lista de operadores obtenida correctamente.";
            }
            catch (System.Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
                _logger.Error(ex, response.Mensaje);
            }

            return response;
        }
    }
}
