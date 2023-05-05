using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries
{
    public class ListarFormatosQuery : IRequest<List<FormatoDTO>>
    {
        public string CodEmpresa { get; set; }
    }

    public class ListarFormatossHandler : IRequestHandler<ListarFormatosQuery, List<FormatoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioSovosFormato _repositorioSovosFormato;

        public ListarFormatossHandler(IMapper mapper, IRepositorioSovosFormato repositorioSovosFormato)
        {
            _mapper = mapper;
            _repositorioSovosFormato = repositorioSovosFormato;
        }

        public async Task<List<FormatoDTO>> Handle(ListarFormatosQuery request, CancellationToken cancellationToken)
        {
            var formatos = await _repositorioSovosFormato.LocListar(request.CodEmpresa);
            var formatosDTO = _mapper.Map<List<FormatoDTO>>(formatos);
            return formatosDTO;
        }
    }
}
