namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaRecepcion
{
    public class GuiaRecepcionDetalleDto
    {
        public string CodProducto { get; set; }
        public long SerieProductoId { get; set; }
        public string NumSerieNueva { get; set; }
        public int Cantidad { get; set; }
        public string CodActivo { get; set; }
        public string Observaciones { get; set; }
    }
}
