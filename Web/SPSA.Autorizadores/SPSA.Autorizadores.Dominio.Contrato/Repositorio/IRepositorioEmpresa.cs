using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioEmpresa
    {
        Task<List<Empresa>> Listar();
        Task<List<Empresa>> ListarOfiplan();
    }
}
