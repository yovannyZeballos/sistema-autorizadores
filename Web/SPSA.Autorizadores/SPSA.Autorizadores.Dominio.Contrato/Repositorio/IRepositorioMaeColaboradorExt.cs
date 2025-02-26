using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioMaeColaboradorExt : IRepositorioGenerico<Mae_ColaboradorExt>
    {
        Task<DataTable> ObtenerColaboradoresPorEmpresaAsync(string codEmpresa);
    }
}
