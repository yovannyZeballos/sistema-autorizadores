using System;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IBCTContexto : IDisposable
	{
		IRepositorioSegSistema RepositorioSegSistema { get; }
		IRepositorioProcesoParametroEmpresa RepositorioProcesoParametroEmpresa { get; }

		bool GuardarCambios();
		Task<bool> GuardarCambiosAsync();
		void Reestablecer();
	}
}
