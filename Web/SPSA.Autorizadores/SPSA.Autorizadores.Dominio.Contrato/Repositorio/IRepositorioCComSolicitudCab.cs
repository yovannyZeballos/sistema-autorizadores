using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioCComSolicitudCab : IRepositorioGenerico<CCom_SolicitudCab>
    {
        Task<DataTable> ObtenerSolicitudCabPorEmpresaAsync(string codEmpresa);
    }
}
