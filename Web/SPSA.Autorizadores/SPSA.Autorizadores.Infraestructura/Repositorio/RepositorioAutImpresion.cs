using SPSA.Autorizadores.Aplicacion.Entities;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioAutImpresion : RepositorioGenerico<BCTContexto, AutImpresion>, IRepositorioAutImpresion
	{
		public RepositorioAutImpresion(BCTContexto context) : base(context) { }

		public BCTContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}
