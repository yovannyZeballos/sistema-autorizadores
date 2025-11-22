using SPSA.Autorizadores.Dominio.Contrato.Dto;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioTrxHeader
	{
		Task<(int cantidadTransacciones, decimal montoFinal)> ObtenerCantidadTransacciones(int local, string date);

		Task<List<DocumentoElectronico>> ListarDocumentosElectronicosSGP(ListarDocumentoElectronicoDto documentoElectronico);
		Task<List<DocumentoElectronico>> ListarDocumentosElectronicosCT3(ListarDocumentoElectronicoDto documentoElectronico);
	}
}
