namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class GuiaDespachoDetalle
    {
        public long Id { get; set; }
        public long GuiaDespachoId { get; set; }
        public string CodProducto { get; set; }
        public long? SerieProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CantidadConfirmada { get; set; }
        public string CodActivo { get; set; }
        public string Observaciones { get; set; }

        public virtual GuiaDespachoCabecera GuiaDespacho { get; set; }
        public virtual Mae_SerieProducto SerieProducto { get; set; }
    }
}
