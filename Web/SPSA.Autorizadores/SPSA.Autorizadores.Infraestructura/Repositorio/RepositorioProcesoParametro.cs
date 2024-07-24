using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioProcesoParametro : IRepositorioProcesoParametro
	{
		public async Task<List<ProcesoParametro>> ListarPorProceso(decimal codProceso)
		{
			using (var bctContexto = new SGPContexto())
			{
				return await bctContexto.ProcesoParametros.Where(x => x.CodProceso == codProceso).ToListAsync();
			}
		}
	}
}
