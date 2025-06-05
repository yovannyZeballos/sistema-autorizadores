using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.ClasificacionMdr;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.ClasificacionMdr
{
    public class ListarMdrClasificacionQuery : IRequest<GenericResponseDTO<List<ListarMdrClasificacionDto>>>
    {
        public string CodOperador { get; set; }
    }

    public class ListarMdrClasificacionHandler : IRequestHandler<ListarMdrClasificacionQuery, GenericResponseDTO<List<ListarMdrClasificacionDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMdrClasificacionHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMdrClasificacionDto>>> Handle(ListarMdrClasificacionQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMdrClasificacionDto>>
            {
                Ok = true,
                Data = new List<ListarMdrClasificacionDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMdrClasificacion
                    .Obtener(x => x.CodOperador == request.CodOperador)
                    .OrderBy(x => x.NomClasificacion)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarMdrClasificacionDto>>(listaEntidades);
                response.Ok = true;
                response.Mensaje = "Lista de clasificaciones obtenida correctamente.";
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
