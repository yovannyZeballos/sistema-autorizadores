using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioMaeCaja : IRepositorioGenerico<Mae_Caja>
    {
        Task<DataTable> ObtenerCajasPorEmpresaAsync(string codEmpresa);
        Task<DataTable> ObtenerCajasPorLocalAsync(string codEmpresa, string codCadena, string codRegion, string codZona, string codLocal);
    }
}
