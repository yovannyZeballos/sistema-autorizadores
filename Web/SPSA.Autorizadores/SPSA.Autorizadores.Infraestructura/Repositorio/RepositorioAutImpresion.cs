using SPSA.Autorizadores.Aplicacion.Entities;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioAutImpresion : RepositorioGenerico<SGPContexto, AutImpresion>, IRepositorioAutImpresion
	{
		public RepositorioAutImpresion(SGPContexto context) : base(context) { }

		public SGPContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}
