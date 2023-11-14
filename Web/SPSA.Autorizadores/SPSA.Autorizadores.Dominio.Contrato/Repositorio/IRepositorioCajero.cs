using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioCajero
	{
		Task<DataTable> ListarCajero(string codigoLocal);
		Task<DataTable> ListarColaboradores(string codEmpresa, string codigoLocal);
		Task Crear(Cajero cajero);
		Task Eliminar(string nroDocumento, string usuario);
		Task<string> GenerarArchivo(string codigoLocal, string tipoSO);
		Task<DataTable> ReporteDiferenciaCajas(string codEmpresa, string codLocal, DateTime fechaInicio, DateTime fechaFin);
		Task<DataTable> ReporteSobres(string codEmpresa, string codLocal, DateTime fechaInicio, DateTime fechaFin);
		Task ActualizarEstado(string codLocal, string codCajero);
		Task<DataTable> ReporteDiferenciaCajasExcel(string codEmpresa, string codLocal, DateTime fechaInicio, DateTime fechaFin);
		Task<DataTable> ListarCajeroVolanteOfiplan(string codEmpresa, decimal codigoLocal);
		Task<DataTable> ListarCajeroVolante(string codEmpresa, string codigoCoordinador);
		Task CrearCajeroVolante(CajeroVolante cajeroVolante);
		Task EliminarCajeroVolante(CajeroVolante cajeroVolante);
	}
}
