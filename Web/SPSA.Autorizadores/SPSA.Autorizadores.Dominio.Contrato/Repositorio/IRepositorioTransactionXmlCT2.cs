using SPSA.Autorizadores.Dominio.Entidades;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioTransactionXmlCT2
	{
		Task<TransactionXmlCT2> ObtenerSpsa();
		Task<TransactionXmlCT2> ObtenerTpsa();
		Task<TransactionXmlCT2> ObtenerHpsa();
		Task<TransactionXmlCT2> ObtenerSpsaCt3();

    }
}
