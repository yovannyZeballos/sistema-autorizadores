using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Linq;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Queries
{
    public class ListarInvActivoQuery : IRequest<GenericResponseDTO<List<ListarInvActivoDTO>>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
    }

    public class ListarInvActivoHandler : IRequestHandler<ListarInvActivoQuery, GenericResponseDTO<List<ListarInvActivoDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarInvActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarInvActivoDTO>>> Handle(ListarInvActivoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarInvActivoDTO>> { Ok = true, Data = new List<ListarInvActivoDTO>() };

            try
            {
                var activos = await _contexto.RepositorioInventarioActivo.Obtener(x => x.CodEmpresa == request.CodEmpresa 
                                                                                    && x.CodCadena == request.CodCadena 
                                                                                    && x.CodRegion == request.CodRegion 
                                                                                    && x.CodZona == request.CodZona
                                                                                    && x.CodLocal == request.CodLocal)
                                                                        .OrderBy(x => x.CodActivo)
                                                                        .ToListAsync();
                response.Data = _mapper.Map<List<ListarInvActivoDTO>>(activos);
                //response.Ok = true;
                //response.Mensaje = "Se ha generado la lista correctamente";
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
