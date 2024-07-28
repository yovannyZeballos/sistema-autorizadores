using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioProcesoParametro : RepositorioGenerico<SGPContexto, ProcesoParametro>, IRepositorioProcesoParametro
	{
		public RepositorioProcesoParametro(SGPContexto context) : base(context) { }

		public SGPContexto Contexto
		{
			get { return _contexto; }
		}
	}
}
