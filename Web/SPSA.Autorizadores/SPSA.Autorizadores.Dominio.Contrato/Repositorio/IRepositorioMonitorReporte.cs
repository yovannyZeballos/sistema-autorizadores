using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioMonitorReporte
    {
        Task<DataTable> ListarMonitorReporte(string codEmpresa, DateTime fecha, string estado);
        Task Crear(MonitorReporte localMonitor);
    }
}
