using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioElectronicJournal
	{
		Task<List<ElectronicJournal>> ListarTransacciones(string cdenaConexion, string date);
	}
}
