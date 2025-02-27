using System.Collections.Generic;

namespace SPSA.Autorizadores.Dominio.Contrato.Auxiliar
{
    public class PagedResult<T>
    {
        public int PageNumber { get; set; }       // Número de página actual
        public int PageSize { get; set; }         // Tamaño de página (registros por página)
        public int TotalRecords { get; set; }     // Total de registros en la consulta
        public int TotalPages { get; set; }       // Total de páginas
        public List<T> Items { get; set; }        // Registros de la página actual
    }
}
