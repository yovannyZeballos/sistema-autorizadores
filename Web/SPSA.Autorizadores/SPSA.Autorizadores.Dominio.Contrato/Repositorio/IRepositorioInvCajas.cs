using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioInvCajas : IRepositorioGenerico<InvCajas>
	{
        Task<DataTable> DescargarInventarioCajas(string codEmpresa);

    }
}
