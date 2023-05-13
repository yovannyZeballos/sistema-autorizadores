using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioEmpresa
    {
        Task<List<Empresa>> Listar();
        Task<List<Empresa>> ListarOfiplan();
        Task<List<Empresa>> ListarMonitor();
        Task<List<Empresa>> LocListarEmpresa();
    }
}
