using System;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{

	public interface ISGPContexto : IContexto
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
		IRepositorioInvCajas RepositorioInvCajas { get; }
		IRepositorioInvTipoActivo RepositorioInvTipoActivo { get; }

		IRepositorioInventarioActivo RepositorioInventarioActivo { get; }
		IRepositorioApertura RepositorioApertura { get; }

		IRepositorioUbiDepartamento RepositorioUbiDepartamento { get; }
		IRepositorioUbiProvincia RepositorioUbiProvincia { get; }
		IRepositorioUbiDistrito RepositorioUbiDistrito { get; }
		IRepositorioProceso RepositorioProceso { get; }
		IRepositorioProcesoEmpresa RepositorioProcesoEmpresa { get; }
		IRepositorioMaeLocalAlterno RepositorioMaeLocalAlterno { get; }
		IRepositorioMonCierreLocal RepositorioMonCierreLocal { get; }
		IRepositorioTmpMonCierreLocal RepositorioTmpMonCierreLocal { get; }
		IRepositorioAutImpresion RepositorioAutImpresion { get; }
		IRepositorioProcesoParametro RepositorioProcesoParametro { get; }
	}
}
