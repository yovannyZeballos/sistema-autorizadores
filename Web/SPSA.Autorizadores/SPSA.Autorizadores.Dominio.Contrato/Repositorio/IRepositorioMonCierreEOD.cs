using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioMonCierreEOD : IRepositorioGenerico<MonCierreEOD>
	{
		Task<List<Mae_Local>> Listar(string codEmpresa, DateTime fecha, int tipo);
	}
}
