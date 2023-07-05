using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioInventarioServidorVirtual
	{
		Task<DataTable> Listar(string codEmpresa, string codFormato, string codLocal, string numeroServidor);
		Task Insertar(InventarioServidorVirtual inventarioServidorVirtual);
		Task EliminarPorIds(string ids);
	}
}
