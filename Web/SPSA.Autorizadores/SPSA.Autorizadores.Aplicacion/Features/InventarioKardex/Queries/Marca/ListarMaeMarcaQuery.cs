using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Marca;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Marca
{
    public class ListarMaeMarcaQuery : IRequest<GenericResponseDTO<List<ListarMaeMarcaDto>>>
    {
    }

    public class ListarMaeMarcaHandler : IRequestHandler<ListarMaeMarcaQuery, GenericResponseDTO<List<ListarMaeMarcaDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeMarcaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeMarcaDto>>> Handle(ListarMaeMarcaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeMarcaDto>>
            {
                Ok = true,
                Data = new List<ListarMaeMarcaDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMaeMarca
                    .Obtener(x => x.IndActivo == "S")
                    .OrderBy(x => x.NomMarca)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarMaeMarcaDto>>(listaEntidades);
                response.Ok = true;
                response.Mensaje = "Lista de registros obtenido correctamente.";
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
