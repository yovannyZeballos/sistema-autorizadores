using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.DataTableSGP.Queries
{
    public class DtUsuariosQuery : IRequest<DataTable>
    {
    }

    public class DtUsuariosHandler : IRequestHandler<DtUsuariosQuery, DataTable>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioDataTable _repositorioDataTable;

        public DtUsuariosHandler(IMapper mapper, IRepositorioDataTable repositorioDataTable)
        {
            _mapper = mapper;
            _repositorioDataTable = repositorioDataTable;
        }

        public async Task<DataTable> Handle(DtUsuariosQuery request, CancellationToken cancellationToken)
        {
            var autorizadoresDataTable = await _repositorioDataTable.ListarUsuarios();
            return autorizadoresDataTable;

        }
    }
}
