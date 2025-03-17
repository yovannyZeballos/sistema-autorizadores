using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSolicitudUsuarioASR : IRepositorioGenerico<ASR_SolicitudUsuario>
    {
        //Task<DataTable> ObtenerColaboradoresPorEmpresaAsync(string codEmpresa);
    }
}
