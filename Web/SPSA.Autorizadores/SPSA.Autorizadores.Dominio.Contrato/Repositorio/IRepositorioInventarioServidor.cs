using SPSA.Autorizadores.Dominio.Entidades;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioInventarioServidor
	{
		Task Insertar(InventarioServidor inventarioServidor);
		Task<DataTable> Listar(string codEmpresa, string codFormato, string codLocal);
		Task<InventarioServidor> Obtener(string codEmpresa, string codFormato, string codLocal, string numServer);
		Task<DataTable> DescargarMaestro(string codEmpresa, string codFormato, string codLocal);
	}
}
