namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class GuiaRecepcionDetalle
    {
        public int Id { get; set; }
        public int GuiaRecepcionId { get; set; }
        public string CodProducto { get; set; }
        public long? SerieProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public string CodActivo { get; set; }
        public string Observaciones { get; set; }

        public virtual GuiaRecepcionCabecera GuiaRecepcion { get; set; }
        public virtual Mae_SerieProducto SerieProducto { get; set; }
        //public virtual Mae_Producto Producto { get; set; }
    }
}
