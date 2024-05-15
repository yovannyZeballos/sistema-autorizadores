using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioTmpMonCierreLocal : IRepositorioGenerico<TmpMonCierreLocal>
	{
		Task BulkInsert(IEnumerable<TmpMonCierreLocal> entities);
		Task Truncate();
	}
}
