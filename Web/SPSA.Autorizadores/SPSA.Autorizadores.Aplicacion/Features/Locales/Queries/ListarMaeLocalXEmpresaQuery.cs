using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{
    public class ListarMaeLocalXEmpresaQuery : IRequest<GenericResponseDTO<List<ListarMaeLocalDTO>>>
    {
        public string CodEmpresa { get; set; }
    }

    public class ListarMaeLocalXEmpresaHandler : IRequestHandler<ListarMaeLocalXEmpresaQuery, GenericResponseDTO<List<ListarMaeLocalDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarMaeLocalXEmpresaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeLocalDTO>>> Handle(ListarMaeLocalXEmpresaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeLocalDTO>> { Ok = true, Data = new List<ListarMaeLocalDTO>() };
            try
            {
                var locales = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa)
                    .OrderBy(x => x.CodLocal)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeLocalDTO>>(locales);
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
