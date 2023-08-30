using SPSA.Autorizadores.Dominio.Entidades;
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
		Task<string> GenerarArchivo(string tipoSO);

	}
}
