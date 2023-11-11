using System;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioCajaCierre
	{
		Task<DataTable> Listar(string codEmpresa, DateTime fechaInicio, DateTime fechafin);
		Task<DataTable> ListarResumen(string codSociedad, int opcion, DateTime fechaInicio, DateTime fechafin);
	}
}
