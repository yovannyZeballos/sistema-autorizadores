namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.GuiaRecepcion
{
    public class GuiaRecepcionDetalleDto
    {
        public long Id { get; set; }
        public int GuiaRecepcionId { get; set; }
        public string CodProducto { get; set; }
        public string DesProducto { get; set; }
        public long SerieProductoId { get; set; }
        public string NumSerieNueva { get; set; }
        public decimal Cantidad { get; set; }
        public string CodActivo { get; set; }
        public string Observaciones { get; set; }
        public string NumSerie { get; set; }
        public bool EsSerializable { get; set; }

    }
}
