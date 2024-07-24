
using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries;
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
    public class ListarUbiDepartamentoQuery : IRequest<GenericResponseDTO<List<ListarUbiDepartamentoDTO>>>
    {
    }

    public class ListarUbiDepartamentoHandler : IRequestHandler<ListarUbiDepartamentoQuery, GenericResponseDTO<List<ListarUbiDepartamentoDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarUbiDepartamentoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarUbiDepartamentoDTO>>> Handle(ListarUbiDepartamentoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarUbiDepartamentoDTO>> { Ok = true, Data = new List<ListarUbiDepartamentoDTO>() };

            try
            {
                var departamentos = await _contexto.RepositorioUbiDepartamento.Obtener()
                    .OrderBy(x => x.NomDepartamento)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarUbiDepartamentoDTO>>(departamentos);
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
