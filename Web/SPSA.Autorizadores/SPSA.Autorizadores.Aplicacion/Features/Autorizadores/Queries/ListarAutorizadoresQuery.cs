using AutoMapper;
using AutoMapper.Mappers;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{

    public class ListarAutorizadoresQuery : IRequest<DataTable>
    {
        public string CodigoLocal { get; set; } = string.Empty;
    }

    public class ListarAutorizadoresHandler : IRequestHandler<ListarAutorizadoresQuery, DataTable>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAutorizadores _repositorioAutorizadores;

        public ListarAutorizadoresHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa , IRepositorioAutorizadores repositorioAutorizadores)
        {
            _mapper = mapper;
            _repositorioAutorizadores = repositorioAutorizadores;
        }

        public async Task<DataTable> Handle(ListarAutorizadoresQuery request, CancellationToken cancellationToken)
        {
            var autorizadoresDataTable = await _repositorioAutorizadores.ListarAutorizador(request.CodigoLocal);
            return autorizadoresDataTable;
            //var dynamicTable = autorizadoresDataTable.AsDynamicEnumerable();
            //return dynamicTable.ToList();
            //var autorizadoresDto = _mapper.Map<List<AutorizadorDTO>>(autorizadores);
            //return autorizadoresDto;
        }
    }
}
