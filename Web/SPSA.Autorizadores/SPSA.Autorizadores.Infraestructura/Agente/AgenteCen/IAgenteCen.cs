using SPSA.Autorizadores.Infraestructura.Agente.AgenteCen.Dto;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Agente.AgenteCen
{
    public interface IAgenteCen
    {
        Task<ConsultaClienteRespuesta> ConsultarCliente(ConsultaClienteRecurso recurso);
        Task InsertarCliente(InsertarClienteRecurso recurso);
	}
}
