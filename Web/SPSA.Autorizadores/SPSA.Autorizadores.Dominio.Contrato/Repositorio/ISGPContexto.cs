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
		IRepositorioMaeHorario RepositorioMaeHorario { get; }
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

		IRepositorioMonCierreEOD RepositorioMonCierreEOD { get; }
		IRepositorioMonCierreEODHist RepositorioMonCierreEODHist { get; }
        IRepositorioMaeColaboradorExt RepositorioMaeColaboradorExt { get; }
        IRepositorioMaeColaboradorInt RepositorioMaeColaboradorInt { get; }
        IRepositorioMaePuesto RepositorioMaePuesto { get; }
        IRepositorioSolicitudUsuarioASR RepositorioSolicitudUsuarioASR { get; }
        IRepositorioCComSolicitudCab RepositorioCComSolicitudCab { get; }
        IRepositorioCComSolicitudDet RepositorioCComSolicitudDet { get; }
        IRepositorioMaeCodComercio RepositorioMaeCodComercio { get; }

        IRepositorioMdrBinesIzipay RepositorioMdrBinesIzipay { get; }
        IRepositorioMdrClasificacion RepositorioMdrClasificacion { get; }
        IRepositorioMdrOperador RepositorioMdrOperador { get; }
        IRepositorioMdrFactorIzipay RepositorioMdrFactorIzipay { get; }
        IRepositorioMdrPeriodo RepositorioMdrPeriodo { get; }

        IRepositorioMaeProveedor RepositorioMaeProveedor { get; }
        IRepositorioMaeProducto RepositorioMaeProducto { get; }
        IRepositorioMaeSerieProducto RepositorioMaeSerieProducto { get; }
        IRepositorioMaeMarca RepositorioMaeMarca { get; }
        IRepositorioMaeAreaGestion RepositorioMaeAreaGestion { get; }
        IRepositorioMovKardex RepositorioMovKardex { get; } //QUITAR

        IRepositorioStockProducto RepositorioStockProducto { get; }
        IRepositorioGuiaRecepcionCabecera RepositorioGuiaRecepcionCabecera { get; }
        IRepositorioGuiaRecepcionDetalle RepositorioGuiaRecepcionDetalle { get; }
        IRepositorioGuiaDespachoCabecera RepositorioGuiaDespachoCabecera { get; }
        IRepositorioGuiaDespachoDetalle RepositorioGuiaDespachoDetalle { get; }

        IRepositorioSrvTipoServidor RepositorioSrvTipoServidor { get; }
        IRepositorioSrvSistemaOperativo RepositorioSrvSistemaOperativo { get; }
        IRepositorioSrvServidor RepositorioSrvServidor { get; }



    }
}
