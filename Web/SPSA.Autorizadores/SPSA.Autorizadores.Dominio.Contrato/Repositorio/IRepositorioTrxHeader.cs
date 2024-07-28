using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioTrxHeader
	{
		Task<(int cantidadTransacciones, decimal montoFinal)> ObtenerCantidadTransacciones(int local, string date);
	}
}
