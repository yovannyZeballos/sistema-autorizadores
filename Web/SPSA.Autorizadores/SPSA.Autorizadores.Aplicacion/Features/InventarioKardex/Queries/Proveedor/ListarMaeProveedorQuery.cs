using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Proveedor;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Proveedor
{
    public class ListarMaeProveedorQuery : IRequest<GenericResponseDTO<List<ListarMaeProveedorDto>>>
    {
    }

    public class ListarMaeProveedorHandler : IRequestHandler<ListarMaeProveedorQuery, GenericResponseDTO<List<ListarMaeProveedorDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeProveedorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeProveedorDto>>> Handle(ListarMaeProveedorQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeProveedorDto>>
            {
                Ok = true,
                Data = new List<ListarMaeProveedorDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMaeProveedor
                    .Obtener(x => x.IndActivo == "S")
                    .OrderBy(x => x.RazonSocial)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarMaeProveedorDto>>(listaEntidades);
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
