using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioSegSistema : RepositorioGenerico<SGPContexto, Seg_Sistema>, IRepositorioSegSistema
	{
		public RepositorioSegSistema(SGPContexto context) : base(context) { }

		public SGPContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}
