using System;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IContexto : IDisposable
	{
		bool GuardarCambios();
		Task<bool> GuardarCambiosAsync();
		void Reestablecer();
		void Rollback();
	}
}
