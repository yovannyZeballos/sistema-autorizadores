using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioTransactionXmlCT2
	{
		Task<(TransactionXmlCT2, List<TransactionXmlCT2>)> Obtener();
	}
}
