using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.PeriodosMdr;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.PeriodosMdr
{
    public class ListarMdrPeriodoQuery : IRequest<GenericResponseDTO<List<ListarMdrPeriodoDto>>>
    {
    }

    public class ListarMdrPeriodoHandler : IRequestHandler<ListarMdrPeriodoQuery, GenericResponseDTO<List<ListarMdrPeriodoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMdrPeriodoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMdrPeriodoDto>>> Handle(ListarMdrPeriodoQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMdrPeriodoDto>>
            {
                Ok = true,
                Data = new List<ListarMdrPeriodoDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMdrPeriodo
                    .Obtener(x=> x.IndActivo == "S")
                    .OrderByDescending(x => x.DesPeriodo)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarMdrPeriodoDto>>(listaEntidades);
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
