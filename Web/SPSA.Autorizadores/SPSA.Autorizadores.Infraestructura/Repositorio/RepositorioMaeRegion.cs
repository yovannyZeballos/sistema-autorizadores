using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioMaeRegion : RepositorioGenerico<BCTContexto, Mae_Region>, IRepositorioMaeRegion
	{
		public RepositorioMaeRegion(BCTContexto context) : base(context) { }

		public BCTContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}
