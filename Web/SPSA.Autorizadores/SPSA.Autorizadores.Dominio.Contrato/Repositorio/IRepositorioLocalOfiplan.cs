using SPSA.Autorizadores.Dominio.Entidades;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioLocalOfiplan
    {
        Task<LocalOfiplan> ObtenerLocal(string codEmpresa, string codSede);
        Task Insertar(LocalOfiplan localOfiplan);
    }
}
