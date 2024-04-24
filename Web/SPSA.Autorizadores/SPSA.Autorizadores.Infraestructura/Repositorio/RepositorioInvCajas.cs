using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	internal class RepositorioInvCajas : RepositorioGenerico<BCTContexto, InvCajas>, IRepositorioInvCajas
	{
		public RepositorioInvCajas(BCTContexto context) : base(context) { }

		public BCTContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}
