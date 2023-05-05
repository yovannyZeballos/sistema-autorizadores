using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSovosInventarioCaja
    {
        Task Insertar(DataTable dt);
    }
}
