using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class InventarioServidor
	{
		public string CodEmpresa { get; private set; }
		public string CodFormato { get; private set; }
		public string CodLocal { get; private set; }
		public string NumServer { get; private set; }
		public string TipoServer { get; private set; }
		public string CodMarca { get; private set; }
		public string CodModelo { get; private set; }
		public string Hostname { get; private set; }
		public string Serie { get; private set; }
		public string Ip { get; private set; }
		public decimal Ram { get; private set; }
		public decimal Hdd { get; private set; }
		public string CodSo { get; private set; }
		public string Replica { get; private set; }
		public string IpRemota { get; private set; }
		public decimal Antiguedad { get; private set; }
		public string Observaciones { get; private set; }
		public string Antivirus { get; private set; }
		public string Usuario { get; private set; }
		public string UsuarioCreacion { get; private set; }
		public string UsuarioModificacion { get; private set; }
		public DateTime? FechaCreacion { get; private set; }
		public DateTime? FechaModificacion { get; private set; }

		public InventarioServidor(string codEmpresa, string codFormato, string codLocal, string numServer, string tipoServer, string codMarca, 
			string codModelo, string hostname, string serie, string ip, decimal ram, decimal hdd, string codSo, string replica, string ipRemota,
			decimal antiguedad, string observaciones, string antivirus, string usuario)
		{
			CodEmpresa = codEmpresa ?? "";
			CodFormato = codFormato ?? "";
			CodLocal = codLocal ?? "";
			NumServer = numServer ?? "";
			TipoServer = tipoServer ?? "";
			CodMarca = codMarca ?? "";
			CodModelo = codModelo ?? "";
			Hostname = hostname ?? "";
			Serie = serie ?? "";
			Ip = ip ?? "";
			Ram = ram;
			Hdd = hdd;
			CodSo = codSo ?? "";
			Replica = replica ?? "";
			IpRemota = ipRemota ?? "";
			Antiguedad = antiguedad;
			Observaciones = observaciones ?? "";
			Antivirus = antivirus ?? "";
			Usuario = usuario ?? "";
		}

		public InventarioServidor(string codEmpresa, string codFormato, string codLocal, string numServer, string tipoServer, string codMarca, 
			string codModelo, string hostname, string serie, string ip, decimal ram, decimal hdd, string codSo, string replica, string ipRemota,
			decimal antiguedad, string observaciones, string antivirus, string usuarioCreacion, string usuarioModificacion, DateTime? fechaCreacion, DateTime? fechaModificacion) 
			: this(codEmpresa, codFormato, codLocal, numServer, tipoServer, codMarca, codModelo, hostname, serie, ip, ram, hdd, codSo, replica, ipRemota, antiguedad, 
				  observaciones, antivirus, "")
		{
			UsuarioCreacion = usuarioCreacion ?? "";
			UsuarioModificacion = usuarioModificacion ?? "";
			FechaCreacion = fechaCreacion;
			FechaModificacion = fechaModificacion;
		}
	}
}
