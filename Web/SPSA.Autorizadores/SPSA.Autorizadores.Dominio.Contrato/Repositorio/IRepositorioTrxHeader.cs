using System;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioTrxHeader
	{
        Task<MonitorTrxInfo> ObtenerCantidadTransacciones(int sucursal, string fecha);
    }
}
