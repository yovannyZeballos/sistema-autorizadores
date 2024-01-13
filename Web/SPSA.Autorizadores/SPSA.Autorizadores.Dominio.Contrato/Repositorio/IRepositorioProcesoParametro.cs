using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioProcesoParametro
	{
		Task<List<ProcesoParametro>> ListarPorProceso(decimal codProceso);
	}
}
