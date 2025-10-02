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
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Servidor
{
    public class ListarTipoServidorQuery : IRequest<GenericResponseDTO<List<SrvTipoServidor>>>
    {
    }

    public class ListarTipoServidorHandler : IRequestHandler<ListarTipoServidorQuery, GenericResponseDTO<List<SrvTipoServidor>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarTipoServidorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<SrvTipoServidor>>> Handle(ListarTipoServidorQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<SrvTipoServidor>>
            {
                Ok = true,
                Data = new List<SrvTipoServidor>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioSrvTipoServidor
                    .Obtener()
                    .OrderBy(x => x.NomTipo)
                    .ToListAsync();

                //response.Data = _mapper.Map<List<ListarMaeSerieProductoDto>>(listaEntidades);
                response.Data = listaEntidades;
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
