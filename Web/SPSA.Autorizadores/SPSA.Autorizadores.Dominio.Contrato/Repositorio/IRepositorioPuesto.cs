using SPSA.Autorizadores.Dominio.Entidades;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioPuesto
    {
        Task<DataTable> Listar(string codEmpresa);
        Task Actualizar(Puesto puesto);
    }
}
