using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioProceso : RepositorioGenerico<BCTContexto, Proceso>, IRepositorioProceso
	{
		public RepositorioProceso(BCTContexto context) : base(context) { }

		public BCTContexto BCTContexto
		{
			get { return _contexto; }
		}
	}
}
