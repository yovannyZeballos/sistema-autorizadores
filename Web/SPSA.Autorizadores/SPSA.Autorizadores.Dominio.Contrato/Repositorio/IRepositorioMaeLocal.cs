using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioMaeLocal : IRepositorioGenerico<Mae_Local>
	{
        Task<DataTable> ObtenerLocalesPorEmpresaAsync(string codEmpresa);
    }
}
