using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioProcesoEmpresa : RepositorioGenerico<BCTContexto, ProcesoEmpresa>, IRepositorioProcesoEmpresa
	{
		public RepositorioProcesoEmpresa(BCTContexto context) : base(context) { }

		public BCTContexto BCTContexto
		{
			get { return _contexto; }
		}
	}
}
