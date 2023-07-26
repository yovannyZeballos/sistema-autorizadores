using System;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class InventarioServidorDTO : RespuestaComunDTO
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
		public string NumServer { get; set; }
		public string TipoServer { get; set; }
		public string CodMarca { get; set; }
		public string CodModelo { get; set; }
		public string Hostname { get; set; }
		public string Serie { get; set; }
		public string Ip { get; set; }
		public decimal Ram { get; set; }
		public decimal Hdd { get; set; }
		public string CodSo { get; set; }
		public string Replica { get; set; }
		public string IpRemota { get; set; }
		public decimal Antiguedad { get; set; }
		public string Observaciones { get; set; }
		public string Antivirus { get; set; }
		public string Usuario { get; set; }
		public string UsuarioCreacion { get; set; }
		public string UsuarioModificacion { get; set; }
		public string FechaCreacion { get; set; }
		public string FechaModificacion { get; set; }
	}
}
