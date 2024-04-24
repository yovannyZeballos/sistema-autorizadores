using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries
{
    public class ListarMaeEmpresaQuery : IRequest<GenericResponseDTO<List<ListarMaeEmpresaDTO>>>
    {
    }

    public class ListarMaeEmpresaHandler : IRequestHandler<ListarMaeEmpresaQuery, GenericResponseDTO<List<ListarMaeEmpresaDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IBCTContexto _contexto;
        private readonly ILogger _logger;
        public ListarMaeEmpresaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeEmpresaDTO>>> Handle(ListarMaeEmpresaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeEmpresaDTO>> { Ok = true, Data = new List<ListarMaeEmpresaDTO>() };

            try
            {
                var empresas = await _contexto.RepositorioMaeEmpresa.Obtener().OrderBy(x=> x.CodEmpresa).ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeEmpresaDTO>>(empresas);
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
