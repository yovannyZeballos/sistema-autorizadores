using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioCComSolicitudDet : IRepositorioGenerico<CCom_SolicitudDet>
    {
        Task<DataTable> ObtenerSolicitudDetPorEmpresaAsync(string codEmpresa);
    }
}
