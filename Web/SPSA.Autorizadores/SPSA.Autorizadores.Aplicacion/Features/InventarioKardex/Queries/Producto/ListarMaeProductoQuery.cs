using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Producto;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Producto
{
    public class ListarMaeProductoQuery : IRequest<GenericResponseDTO<List<ListarMaeProductoDto>>>
    {
    }

    public class ListarMaeProductoHandler : IRequestHandler<ListarMaeProductoQuery, GenericResponseDTO<List<ListarMaeProductoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeProductoDto>>> Handle(ListarMaeProductoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeProductoDto>>
            {
                Ok = true,
                Data = new List<ListarMaeProductoDto>()
            };

            try
            {
                var productos = await _contexto.RepositorioMaeProducto
                   .Obtener(x => x.IndActivo == "S")
                   .Include(x => x.Marca)              // para usar p.Marca.NomMarca
                   .OrderBy(x => x.DesProducto)
                   .Select(p => new ListarMaeProductoDto
                   {
                       CodProducto = p.CodProducto,
                       DesProducto = p.DesProducto,
                       MarcaId = p.MarcaId,
                       NomMarca = p.Marca.NomMarca,   // ← aquí se llena
                       TipProducto = p.TipProducto,
                       AreaGestionId = p.AreaGestionId,
                       IndSerializable = p.IndSerializable,
                       IndActivo = p.IndActivo,
                       StkMinimo = p.StkMinimo,
                       StkMaximo = p.StkMaximo,
                       NomModelo = p.NomModelo,
                       UsuCreacion = p.UsuCreacion,
                       FecCreacion = p.FecCreacion,
                       UsuModifica = p.UsuModifica,
                       FecModifica = p.FecModifica,
                       UsuElimina = p.UsuElimina,
                       FecElimina = p.FecElimina
                   })
                   .ToListAsync(cancellationToken);
                response.Data = productos;

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
