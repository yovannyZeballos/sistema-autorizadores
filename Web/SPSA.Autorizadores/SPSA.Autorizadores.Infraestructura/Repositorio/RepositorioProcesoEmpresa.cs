using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioProcesoEmpresa : RepositorioGenerico<SGPContexto, ProcesoEmpresa>, IRepositorioProcesoEmpresa
	{
		public RepositorioProcesoEmpresa(SGPContexto context) : base(context) { }

		public SGPContexto BCTContexto
		{
			get { return _contexto; }
		}
	}
}
