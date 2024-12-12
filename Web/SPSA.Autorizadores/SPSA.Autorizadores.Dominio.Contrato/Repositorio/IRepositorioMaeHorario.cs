using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioMaeHorario : IRepositorioGenerico<Mae_Horario>
    {
        Task<DataTable> ObtenerHorariosPorEmpresaAsync(string codEmpresa);
    }
}
