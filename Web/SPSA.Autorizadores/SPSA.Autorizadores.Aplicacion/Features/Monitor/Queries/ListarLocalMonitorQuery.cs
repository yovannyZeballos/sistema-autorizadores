using MediatR;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
    public class ListarLocalMonitorQuery : IRequest<DataTable>
    {
        public string CodEmpresa { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }

    public class ListarLocalMonitorHandler : IRequestHandler<ListarLocalMonitorQuery, DataTable>
    {
        private readonly IRepositorioMonitorReporte _repositorioLocalMonitor;

        public ListarLocalMonitorHandler(IRepositorioMonitorReporte repositorioLocalMonitor)
        {
            _repositorioLocalMonitor = repositorioLocalMonitor;
        }

        public async Task<DataTable> Handle(ListarLocalMonitorQuery request, CancellationToken cancellationToken)
        {
            var autorizadoresDataTable = await _repositorioLocalMonitor.ListarMonitorReporte(request.CodEmpresa, request.Fecha, request.Estado);
            return autorizadoresDataTable;
        }
    }
}
