using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class ASR_UsuarioListado
    {
		public int NumSolicitud { get; set; }
		public string CodLocal { get; set; }
		public string NomLocal { get; set; }
		public string CodColaborador { get; set; }
		public string CodUsuarioAsr { get; set; }
		public string NoApelPate { get; set; }
		public string NoApelMate { get; set; }
		public string NoTrab { get; set; }
		public string TipAccion { get; set; }
		public string TipUsuario { get; set; }
		public string TipColaborador { get; set; }
		public string DePuesTrab { get; set; }
		public string IndAprobado { get; set; }
		public string UsuSolicita { get; set; }
		public string UsuSolNoApelPate { get; set; }
		public string UsuSolNoApelMate { get; set; }
		public string UsuSolNoTrab { get; set; }
		public DateTime? FecSolicita { get; set; }
		public string UsuAprobacion { get; set; }
		public string UsuAprNoApelPate { get; set; }
		public string UsuAprNoApelMate { get; set; }
		public string UsuAprNoTrab { get; set; }
		public DateTime? FecAprobacion { get; set; }
		public string Motivo { get; set; }
		public string NumDocumentoIdentidad { get; set; }
		public int TotalRegistros { get; set; }

		public override string ToString()
		{
			return $"{NumSolicitud}{CodLocal}{NomLocal}{CodColaborador}{CodUsuarioAsr}{NoApelPate}{NoApelMate}{NoTrab}{TipAccion}{TipUsuario}{TipColaborador}{DePuesTrab}{IndAprobado}{UsuSolicita}{UsuSolNoApelPate}{UsuSolNoApelMate}{UsuSolNoTrab}{FecSolicita?.ToString("yyyyMMdd")}{UsuAprobacion}{UsuAprNoApelPate}{UsuAprNoApelMate}{UsuAprNoTrab}{FecAprobacion?.ToString("yyyyMMdd")}{Motivo}{NumDocumentoIdentidad}{TotalRegistros}";
		}
	}
}