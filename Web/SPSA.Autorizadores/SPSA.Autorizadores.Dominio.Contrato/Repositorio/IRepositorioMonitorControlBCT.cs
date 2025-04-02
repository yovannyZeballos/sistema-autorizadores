using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioMonitorControlBCT
	{
		Task<List<MonitorControlBCT>> ObtenerHorarioSucursalCT2(string fecha, int local);		
		Task<List<MonitorControlBCT>> ObtenerHorarioSucursalCT2Tpsa(string fecha, int local);
		Task<List<MonitorControlBCT>> ObtenerHorarioSucursalCT2Hpsa(string fecha, int local);
        Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCT(string fecha, int sucursal);
        Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCTTpsa(string fecha, int sucursal);
        Task<List<MonitorControlBCT>> ObtenerHorarioSucursalBCTHpsa(string fecha, int sucursal);


	}
}
