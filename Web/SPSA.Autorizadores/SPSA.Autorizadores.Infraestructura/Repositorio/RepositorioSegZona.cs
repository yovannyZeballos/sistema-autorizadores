using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioSegZona : RepositorioGenerico<BCTContexto, Seg_Zona>, IRepositorioSegZona
	{
		public RepositorioSegZona(BCTContexto context) : base(context) { }

		public BCTContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}
	}
}