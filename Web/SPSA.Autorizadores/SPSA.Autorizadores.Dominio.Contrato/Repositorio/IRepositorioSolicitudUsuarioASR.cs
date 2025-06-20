using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSolicitudUsuarioASR : IRepositorioGenerico<ASR_SolicitudUsuario>
    {
        Task<List<ASR_UsuarioListado>> ListarUsuarios(string usuarioLogin, string codLocal, 
            string usuAprobacion, string tipAccion, string indAprobado, string CodEmpresa,
            int numeroPagina, int tamañoPagina);
		Task<List<ASR_UsuarioListado>> ListarSolicitudes(string usuarioLogin, string tipoUsuario, string CodEmpresa, int numeroPagina, int tamañoPagina);
        Task ActualizarMotivoRechazo(int numSolicitud, string motivo, string estado);
        Task AprobarSolicitud(ASR_Usuario usuario);
		Task<List<ASR_UsuarioArchivo>> ListarArchivos(string tipUsuario);
		Task ActualizarFlagEnvio(long numSolicitud, string flagEnvio);

        // Oracle
        Task<List<int>> ObtenerLocalesPorProcesarAsyncOracleSpsa();
        Task<List<ASR_CajeroPaso>> ObtenerCajerosPorProcesarAsyncOracleSpsa(int codLocal);
        Task ActualizarFlagProcesadoAsyncOracleSpsa(int codLocal, string codCajero, string flagProcesado);
        Task<(int, string)> NuevoCajeroAsyncOracleSpsa(ASR_CajeroPaso cajero);
        Task<(int, string)> EliminarCajeroaAsyncOracleSpsa(ASR_CajeroPaso cajero);

        // Postgress
        Task<List<int>> ObtenerLocalesPorProcesarAsync(int codPais, int codComercio);
        Task<List<ASR_CajeroPaso>> ObtenerCajerosPorProcesarAsync(int codPais, int codComercio, int codLocal);
        Task ActualizarFlagProcesadoAsync(int codPais, int codComercio, int codLocal, string codCajero, string flagProcesado);
        Task<(int, string)> NuevoCajeroAsync(ASR_CajeroPaso cajero);
        Task<(int, string)> EliminarCajeroSpsaAsync(ASR_CajeroPaso cajero);

    }
}
