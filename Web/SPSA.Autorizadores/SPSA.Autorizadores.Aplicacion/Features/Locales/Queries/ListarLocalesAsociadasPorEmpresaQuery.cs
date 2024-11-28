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
using System.Data.Entity;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{
    public class ListarLocalesAsociadasPorEmpresaQuery : IRequest<GenericResponseDTO<List<ListarLocalDTO>>>
    {
        public string CodUsuario { get; set; }
        public string CodEmpresa { get; set; }
    }

    public class ListarLocalesAsociadasPorEmpresaHandler : IRequestHandler<ListarLocalesAsociadasPorEmpresaQuery, GenericResponseDTO<List<ListarLocalDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarLocalesAsociadasPorEmpresaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarLocalDTO>>> Handle(ListarLocalesAsociadasPorEmpresaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarLocalDTO>> { Ok = true, Data = new List<ListarLocalDTO>() };

            try
            {
                var Locales = await _contexto.RepositorioSegLocal
                    .Obtener(x => x.CodUsuario == request.CodUsuario
                            && x.CodEmpresa == request.CodEmpresa)
                    .Include(x => x.Mae_Local)
                    .OrderBy(x => x.CodLocal)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarLocalDTO>>(Locales);
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensaje = "Ocurrió un error al listar las Locales asociadas";
                _logger.Error(ex, response.Mensaje);
            }
            return response;
        }
    }
}
