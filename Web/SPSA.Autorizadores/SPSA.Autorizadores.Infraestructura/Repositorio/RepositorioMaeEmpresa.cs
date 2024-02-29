using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	/// <summary>
	/// Implementa los métodos definidos en la interfaz <see cref="IRepositorioMaeEmpresa"/>.
	/// </summary>
	public class RepositorioMaeEmpresa : RepositorioGenerico<BCTContexto, Mae_Empresa>, IRepositorioMaeEmpresa
	{
		/// <summary>
		/// Inicializa una nueva instancia de la clase <see cref="RepositorioMaeEmpresa"/>.
		/// </summary>
		/// <param name="context">El contexto de la base de datos.</param>
		public RepositorioMaeEmpresa(BCTContexto context) : base(context) { }

		/// <summary>
		/// Obtiene el contexto de la base de datos.
		/// </summary>
		public BCTContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}