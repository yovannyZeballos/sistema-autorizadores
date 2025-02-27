using System;

namespace SPSA.Autorizadores.Aplicacion.Features.Puestos.DTOs
{
    public class ObtenerMaePuestoDTO
    {
        public string CodEmpresa { get; set; }
        public string CodPuesto { get; set; }
        public string DesPuesto { get; set; }
        public string IndAutAut { get; set; }
        public string IndAutOpe { get; set; }
        public string IndManAut { get; set; }
        public string IndManOpe { get; set; }
        public DateTime? FecAsigna { get; set; }
        public string UsuAsigna { get; set; }
        public DateTime? FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecElimina { get; set; }
        public string UsuElimina { get; set; }
    }
}
