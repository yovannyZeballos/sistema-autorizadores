using SPSA.Autorizadores.Infraestructura.Agente.AgenteAxteroid.Dto;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Agente.AgenteAxteroid
{
    public interface IAgenteAxteroid
    {
		Task<ConsultaDocumentoElectronicoRespuesta> ConsultarDocumento(ConsultaDocumentoElectronicoRecurso consultaDocumentoElectronicoRecurso);

		Task<byte[]> DescargarDocumento(string url);

	}
}
