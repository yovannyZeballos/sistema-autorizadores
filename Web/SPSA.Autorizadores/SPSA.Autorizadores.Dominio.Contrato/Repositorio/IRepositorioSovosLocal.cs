using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSovosLocal
    {
        Task<List<SovosLocal>> ListarMonitor(string codEmpresa, DateTime fecha);
        Task Crear(SovosLocal sovosLocal);
        Task<DataTable> ListarPorEmpresa(string codEmpresa, string codFormato);
        Task<SovosLocal> ObtenerLocal(string codEmpresa, string codFormato, string codLocal);
        Task<DataTable> DescargarMaestro(string codEmpresa, string codFormato);

    }
}
