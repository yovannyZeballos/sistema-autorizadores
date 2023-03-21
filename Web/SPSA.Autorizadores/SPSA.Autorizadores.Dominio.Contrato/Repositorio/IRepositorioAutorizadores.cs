using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioAutorizadores
    {
        Task<DataTable> ListarColaboradores(string codigoLocal, string codigoEmpresa);
        Task<DataTable> ListarAutorizador(string codigoLocal);
        Task<DataTable> ListarColaboradoresCesados();
        Task Crear(Autorizador autorizador);
        Task Eliminar(Autorizador autorizador);
        Task ActualizarEstadoArchivo(Autorizador autorizador);
        Task<string> GenerarArchivo(string tipoSO);
        Task<DataTable> ListarColaboradoresMass(string codEmpresa);


    }
}
