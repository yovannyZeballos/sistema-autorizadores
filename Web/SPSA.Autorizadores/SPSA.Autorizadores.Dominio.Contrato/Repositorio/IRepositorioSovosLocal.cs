using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSovosLocal
    {
        Task<List<SovosLocal>> Listar(string codEmpresa, DateTime fecha);
    }
}
