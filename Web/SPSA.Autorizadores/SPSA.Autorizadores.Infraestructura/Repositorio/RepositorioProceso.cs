using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioProceso : RepositorioGenerico<SGPContexto, Proceso>, IRepositorioProceso
	{
		public RepositorioProceso(SGPContexto context) : base(context) { }

		public SGPContexto BCTContexto
		{
			get { return _contexto; }
		}
	}
}
