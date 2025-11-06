using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
    public class ListarMaeProductoPorLocalQuery : IRequest<GenericResponseDTO<List<ListarMaeProductoDto>>>
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
    }

    public class ListarMaeProductoPorLocalHandler : IRequestHandler<ListarMaeProductoPorLocalQuery, GenericResponseDTO<List<ListarMaeProductoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMaeProductoPorLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeProductoDto>>> Handle(ListarMaeProductoPorLocalQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeProductoDto>>
            {
                Ok = true,
                Data = new List<ListarMaeProductoDto>()
            };

            try
            {

                var stockPorProducto = _contexto.RepositorioStockProducto
                    .Obtener(sp => sp.CodEmpresa == request.CodEmpresa && sp.CodLocal == request.CodLocal)
                    .GroupBy(sp => sp.CodProducto)
                    .Select(g => new
                    {
                        CodProducto = g.Key,
                        StkDisponible = g.Sum(x => (decimal?)x.StkDisponible ?? 0m)
                    });

                var query = from s in stockPorProducto
                            join p in _contexto.RepositorioMaeProducto.Obtener(p => p.IndActivo == "S")
                              on s.CodProducto equals p.CodProducto
                            select new ListarMaeProductoDto
                            {
                                CodProducto = p.CodProducto,
                                DesProducto = p.DesProducto,
                                MarcaId = p.MarcaId,
                                NomMarca = p.Marca.NomMarca,
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
                                FecElimina = p.FecElimina,
                                // dato clave:
                                StkDisponible = s.StkDisponible
                            };

                response.Data = await query
                    .OrderBy(x => x.DesProducto)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

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
