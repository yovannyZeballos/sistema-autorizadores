using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioDataTable
    {
        Task<DataTable> ListarUsuarios();
    }
}
