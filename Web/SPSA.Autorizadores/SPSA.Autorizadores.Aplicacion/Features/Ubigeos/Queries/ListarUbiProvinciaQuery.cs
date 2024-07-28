
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
    public class ListarUbiProvinciaQuery : IRequest<GenericResponseDTO<List<ListarUbiProvinciaDTO>>>
    {
        public string CodDepartamento { get; set; }
    }

    public class ListarUbiProvinciaHandler : IRequestHandler<ListarUbiProvinciaQuery, GenericResponseDTO<List<ListarUbiProvinciaDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarUbiProvinciaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarUbiProvinciaDTO>>> Handle(ListarUbiProvinciaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarUbiProvinciaDTO>> { Ok = true, Data = new List<ListarUbiProvinciaDTO>() };

            try
            {
                var provincias = await _contexto.RepositorioUbiProvincia.Obtener(x => x.CodDepartamento == request.CodDepartamento)
                    .OrderBy(x => x.NomProvincia)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarUbiProvinciaDTO>>(provincias);
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
