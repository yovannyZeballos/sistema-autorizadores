using SPSA.Autorizadores.Dominio.Entidades;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioMonCierreLocal : IRepositorioGenerico<MonCierreLocal>
	{
		Task InsertarActualizar();
	}
}
