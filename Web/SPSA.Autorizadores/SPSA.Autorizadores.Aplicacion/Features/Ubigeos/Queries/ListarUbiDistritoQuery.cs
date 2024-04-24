using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Linq;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Ubigeos.Queries
{
    public class ListarUbiDistritoQuery : IRequest<GenericResponseDTO<List<ListarUbiDistritoDTO>>>
    {
        public string CodDepartamento { get; set; }
        public string CodProvincia { get; set; }
    }

    public class ListarUbiDistritoHandler : IRequestHandler<ListarUbiDistritoQuery, GenericResponseDTO<List<ListarUbiDistritoDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IBCTContexto _contexto;
        private readonly ILogger _logger;

        public ListarUbiDistritoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarUbiDistritoDTO>>> Handle(ListarUbiDistritoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarUbiDistritoDTO>> { Ok = true, Data = new List<ListarUbiDistritoDTO>() };

            try
            {
                var distritos = await _contexto.RepositorioUbiDistrito.Obtener(x => x.CodDepartamento == request.CodDepartamento && x.CodProvincia == request.CodProvincia)
                    .OrderBy(x => x.NomDistrito)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarUbiDistritoDTO>>(distritos);
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
