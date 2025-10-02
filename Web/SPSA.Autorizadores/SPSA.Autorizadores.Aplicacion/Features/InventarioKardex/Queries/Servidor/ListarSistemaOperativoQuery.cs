using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
    public class ListarSistemaOperativoQuery : IRequest<GenericResponseDTO<List<SrvSistemaOperativo>>>
    {
    }

    public class ListarSistemaOperativoHandler : IRequestHandler<ListarSistemaOperativoQuery, GenericResponseDTO<List<SrvSistemaOperativo>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarSistemaOperativoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<SrvSistemaOperativo>>> Handle(ListarSistemaOperativoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<SrvSistemaOperativo>>
            {
                Ok = true,
                Data = new List<SrvSistemaOperativo>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioSrvSistemaOperativo
                    .Obtener()
                    .OrderBy(x => x.NomSo)
                    .ToListAsync();

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
