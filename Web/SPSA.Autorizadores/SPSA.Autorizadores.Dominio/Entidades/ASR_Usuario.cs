using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class ASR_Usuario
    {
		public int NumSolicitud { get; set; }
		public int CodLocalAlterno { get; set; }
		public string CodColaborador { get; set; }
		public string CodUsuarioAsr { get; set; }
		public string TipUsuario { get; set; }
		public string TipColaborador { get; set; }
		public string IndActivo { get; set; }
		public string FlgEnvio { get; set; }
		public DateTime? FecEnvio { get; set; }
		public string UsuAutoriza { get; set; }
		public DateTime? FecAutoriza { get; set; }
		public string UsuCreacion { get; set; }
		public DateTime? FecCreacion { get; set; }
		public string UsuElimina { get; set; }
		public DateTime? FecElimina { get; set; }
		public DateTime FecSistema { get; set; }
	}
}