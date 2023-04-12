using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSovosFormato
    {
        Task<List<SovosFormato>> LocListar(string codEmpresa);
    }
}
