using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using SPSA.Autorizadores.Aplicacion.Logger;
using Serilog;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Data.Entity;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Cadenas.Queries
{
    public class ListarMaeCadenaQuery : IRequest<GenericResponseDTO<List<ListarMaeCadenaDTO>>>
    {
        public string CodEmpresa { get; set; }
    }

    public class ListarMaeCadenaHandler : IRequestHandler<ListarMaeCadenaQuery, GenericResponseDTO<List<ListarMaeCadenaDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarMaeCadenaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeCadenaDTO>>> Handle(ListarMaeCadenaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeCadenaDTO>> { Ok = true, Data = new List<ListarMaeCadenaDTO>() };

            try
            {
                var empresas = await _contexto.RepositorioMaeCadena.Obtener(x=> x.CodEmpresa == request.CodEmpresa)
                    .OrderBy(x=> x.CodCadena)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeCadenaDTO>>(empresas);
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
