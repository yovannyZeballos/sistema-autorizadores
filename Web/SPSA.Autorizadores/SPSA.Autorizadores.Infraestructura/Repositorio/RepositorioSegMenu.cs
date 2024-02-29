using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioSegMenu : RepositorioGenerico<BCTContexto, Seg_Menu>, IRepositorioSegMenu
	{
		public RepositorioSegMenu(BCTContexto context) : base(context) { }

		public BCTContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}