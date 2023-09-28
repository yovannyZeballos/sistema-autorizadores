using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioProcesos
	{
		Task<List<ListBox>> ListarListBox(string usuario);
		Task<DataTable> ListarGrilla(int opcion, string codEmpresa, string codLocal, DateTime fechaInicio, DateTime fechaFin);
	}
}
