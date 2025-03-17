using System;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.DTOs
{
    public class ListarSolictudUsuarioDTO
    {
        public int NumSolicitud { get; set; }
        public string CodLocalAlterno { get; set; }
        public string CodColaborador { get; set; }
        public string TipUsuario { get; set; }
        public string TipColaborador { get; set; }
        public string UsuSolicita { get; set; }
        public DateTime FecSolicita { get; set; }
        public string TipAccion { get; set; }
        public string UsuAprobacion { get; set; }
        public DateTime? FecAprobacion { get; set; }
        public string IndAprobado { get; set; }
        public string Motivo { get; set; }
        public string UsuElimina { get; set; }
        public DateTime? FecElimina { get; set; }
    }
}
