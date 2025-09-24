namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaDespacho
{
    public class GuiaDespachoDetalleDto
    {
        public long Id { get; set; }
        public long GuiaDespachoId { get; set; }
        public string CodProducto { get; set; }
        public long? SerieProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CantidadConfirmada { get; set; } = 0m;
        public string CodActivo { get; set; }
        public string Observaciones { get; set; }
        public string NumSerie { get; set; }
        public bool EsSerializable { get; set; } 

    }
}
