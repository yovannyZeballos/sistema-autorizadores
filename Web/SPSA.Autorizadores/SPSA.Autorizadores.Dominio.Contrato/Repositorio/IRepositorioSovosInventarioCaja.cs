using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioSovosInventarioCaja
    {
        Task InsertarMasivo(DataTable dt);
        Task Insertar(SovosCajaInventario sovosCajaInventario);
        Task<List<CaracteristicaCaja>> ListarCaracteristicas(string codEmpresa, int tipo);
        Task<DataTable> Listar();
        Task<SovosCajaInventario> Obtener(string codEmpresa, string codFormato, string codLocal, decimal numPos);
		Task<DataTable> DescargarMaestro(string codEmpresa, string codFormato, string codLocal);

	}
}
