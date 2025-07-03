using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.DTOs.Bines;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.Bines
{
    public class ListarMdrBinesIzipayQuery : IRequest<GenericResponseDTO<List<ListarMdrBinesDto>>>
    {
        public string CodEmpresa { get; set; }
        public int CodPeriodo { get; set; }
    }

    public class ListarMdrBinesIzipayHandler : IRequestHandler<ListarMdrBinesIzipayQuery, GenericResponseDTO<List<ListarMdrBinesDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;

        public ListarMdrBinesIzipayHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMdrBinesDto>>> Handle(ListarMdrBinesIzipayQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMdrBinesDto>>
            {
                Ok = true,
                Data = new List<ListarMdrBinesDto>()
            };

            try
            {
                var listaEntidades = await _contexto.RepositorioMdrBinesIzipay
                    .Obtener(x =>
                        x.CodEmpresa == request.CodEmpresa &&
                        x.CodPeriodo == request.CodPeriodo)
                    .OrderBy(x => x.NumBin6)
                    .ToListAsync(cancellationToken);

                response.Data = _mapper.Map<List<ListarMdrBinesDto>>(listaEntidades);
                response.Ok = true;
                response.Mensaje = "Lista de BINES obtenida correctamente.";
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
