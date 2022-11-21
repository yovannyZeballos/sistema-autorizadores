using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioLocal
    {
        Task<List<Local>> ListarXEmpresa(string ruc);
        Task<Local> ObtenerCarteleria(string codLocal);
        Task<Local> Obtener(int codigo);
    }
}
