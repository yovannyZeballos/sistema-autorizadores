using System.Collections.Generic;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioMdrBinesIzipay : IRepositorioGenerico<Mdr_BinesIzipay>
    {
        /// <summary>
        /// Llama a la función SF_MDR_CONSOLIDADO_BINES en PostgreSQL y retorna 
        /// la lista de registros de tipo ConsolidadoBinesDto.
        /// </summary>
        /// <param name="codEmpresa">Código de empresa (e.g. "03")</param>
        /// <param name="numAno">Año (e.g. "2025")</param>
        /// <returns>Lista de objetos ConsolidadoBinesDto</returns>
        Task<List<Mdr_BinesIzipay>> ObtenerConsolidadoBinesAsync(string codEmpresa, string numAno);

    }
}
