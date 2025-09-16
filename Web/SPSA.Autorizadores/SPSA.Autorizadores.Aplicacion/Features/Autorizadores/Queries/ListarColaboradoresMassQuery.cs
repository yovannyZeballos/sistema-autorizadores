using MediatR;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{

    public class ListarColaboradoresMassQuery : IRequest<DataTable>
    {
        public string CodigoEmpresa { get; set; }
    }

    //public class ListarColaboradoresMassHandler : IRequestHandler<ListarColaboradoresMassQuery, DataTable>
    //{
    //    private readonly IRepositorioAutorizadores _repositorioAutorizadores;

    //    public ListarColaboradoresMassHandler(IRepositorioAutorizadores repositorioAutorizadores)
    //    {
    //        _repositorioAutorizadores = repositorioAutorizadores;
    //    }

    //    public async Task<DataTable> Handle(ListarColaboradoresMassQuery request, CancellationToken cancellationToken)
    //    {
    //        var autorizadoresDataTable = await _repositorioAutorizadores.ListarColaboradoresMass(request.CodigoEmpresa);
    //        return autorizadoresDataTable;
    //    }
    //}
}
