using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	internal class RepositorioInvCajas : RepositorioGenerico<SGPContexto, InvCajas>, IRepositorioInvCajas
	{
		public RepositorioInvCajas(SGPContexto context) : base(context) { }

		public SGPContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}
