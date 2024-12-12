using System;

namespace SPSA.Autorizadores.Aplicacion.Features.Horarios.DTOs
{
    public class ObtenerMaeHorarioDTO
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public int NumDia { get; set; }
        public string CodDia { get; set; }
        public string HorOpen { get; set; }
        public string HorClose { get; set; }
        public string MinLmt { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime FecModifica { get; set; }
    }
}
