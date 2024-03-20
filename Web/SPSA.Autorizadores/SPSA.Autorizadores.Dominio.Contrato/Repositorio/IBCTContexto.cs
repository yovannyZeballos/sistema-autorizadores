using System;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IBCTContexto : IDisposable
	{
		IRepositorioSegSistema RepositorioSegSistema { get; }
		IRepositorioProcesoParametroEmpresa RepositorioProcesoParametroEmpresa { get; }
		IRepositorioSegUsuario RepositorioSegUsuario { get; }		
		IRepositorioSegEmpresa RepositorioSegEmpresa { get; }
		IRepositorioSegCadena RepositorioSegCadena { get; }
		IRepositorioSegRegion RepositorioSegRegion { get; }
		IRepositorioSegZona RepositorioSegZona { get; }
		IRepositorioSegLocal RepositorioSegLocal { get; }
		IRepositorioSegPerfil RepositorioSegPerfil { get; }
		IRepositorioSegPerfilUsuario RepositorioSegPerfilUsuario { get; }
		IRepositorioSegMenu RepositorioSegMenu { get; }
		IRepositorioSegPerfilMenu RepositorioSegPerfilMenu { get; }

        IRepositorioMaeEmpresa RepositorioMaeEmpresa { get; }
        IRepositorioMaeCadena RepositorioMaeCadena { get; }
        IRepositorioMaeRegion RepositorioMaeRegion { get; }
        IRepositorioMaeZona RepositorioMaeZona { get; }
        IRepositorioMaeLocal RepositorioMaeLocal { get; }
        IRepositorioMaeCaja RepositorioMaeCaja { get; }

        IRepositorioInventarioActivo RepositorioInventarioActivo { get; }

        bool GuardarCambios();
		Task<bool> GuardarCambiosAsync();
		void Reestablecer();
	}
}
