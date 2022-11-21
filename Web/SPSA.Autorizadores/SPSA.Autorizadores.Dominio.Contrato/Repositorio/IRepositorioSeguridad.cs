using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSeguridad
    {
        Task<SegUsuario> Login(int codigoSistema, string usuario, string password, string abrSistema);
        Task<List<SegSucursalxUsr>> ObtenerLocalXUsuario(string login);
    }
}
