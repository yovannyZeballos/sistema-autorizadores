using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioMonitorControlBCT
	{
		Task<List<MonitorControlBCT>> ObtenerHorarioSucursalCT2(string fecha, int local);
		Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCT(string fecha, int local);
	}
}
