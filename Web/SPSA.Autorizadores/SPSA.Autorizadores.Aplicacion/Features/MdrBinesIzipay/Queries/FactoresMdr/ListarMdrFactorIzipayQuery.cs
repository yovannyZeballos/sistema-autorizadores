using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.FactoresMdr
{
    public class ListarMdrFactorIzipayQuery : IRequest<GenericResponseDTO<List<ListarMdrFactorDto>>>
    {
        public string CodEmpresa { get; set; }
        public string NumAno { get; set; }
    }

    public class ListarMdrFactorIzipayHandler : IRequestHandler<ListarMdrFactorIzipayQuery, GenericResponseDTO<List<ListarMdrFactorDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMdrFactorIzipayHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMdrFactorDto>>> Handle(ListarMdrFactorIzipayQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMdrFactorDto>>
            {
                Ok = true,
                Data = new List<ListarMdrFactorDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMdrFactorIzipay
                    .Obtener(x =>
                        x.CodEmpresa == request.CodEmpresa &&
                        x.NumAno == request.NumAno)
                    .OrderBy(x => x.CodOperador)
                    .ThenBy(x => x.CodClasificacion)
                    .ToListAsync();

                response.Data = _mapper.Map<List<ListarMdrFactorDto>>(listaEntidades);
                response.Ok = true;
                response.Mensaje = "Lista de Factores MDR obtenida correctamente.";
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
