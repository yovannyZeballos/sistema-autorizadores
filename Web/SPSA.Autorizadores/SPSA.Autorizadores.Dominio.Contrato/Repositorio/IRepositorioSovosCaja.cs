using SPSA.Autorizadores.Dominio.Entidades;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSovosCaja
    {
        Task Crear(SovosCaja sovosCaja);
        Task<DataTable> ListarPorLocal(string codEmpresa, string codFormato, string codLocal);
        Task Eliminar(string codEmpresa, string codFormato, string codLocal, string cajas);

    }
}
