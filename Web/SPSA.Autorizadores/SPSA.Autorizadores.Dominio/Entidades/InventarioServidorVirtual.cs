using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class InventarioServidorVirtual : DatosAuditoria
	{
		public int Id { get; private set; }
		public string CodEmpresa { get; private set; }
		public string CodFormato { get; private set; }
		public string CodLocal { get; private set; }
		public string NumServer { get; private set; }
		public string Tipo { get; private set; }
		public decimal Ram { get; private set; }
		public decimal Cpu { get; private set; }
		public decimal Hdd { get; private set; }
		public string So { get; private set; }
		public string Usuario { get; private set; }

		public InventarioServidorVirtual(int id, string codempresa, string codFormato, string codLocal, string numServer,
			string tipo, decimal ram, decimal cpu, decimal hdd, string so, string usuario) : base("", "", null, null)
		{
			Id = id;
			CodEmpresa = codempresa;
			CodFormato = codFormato;
			CodLocal = codLocal;
			NumServer = numServer;
			Tipo = tipo;
			Ram = ram;
			Cpu = cpu;
			Hdd = hdd;
			So = so;
			Usuario = usuario;
		}

		public InventarioServidorVirtual(int id, string codempresa, string codFormato, string codLocal, string numServer,
			string tipo, decimal ram, decimal cpu, decimal hdd, string so, string usuarioCreacion, string usuarioModificacion,
			DateTime? fechaCreacion, DateTime? fechaModificacion) : base(usuarioCreacion, usuarioModificacion, fechaCreacion, fechaModificacion)
		{
			Id = id;
			CodEmpresa = codempresa;
			CodFormato = codFormato;
			CodLocal = codLocal;
			NumServer = numServer;
			Tipo = tipo;
			Ram = ram;
			Cpu = cpu;
			Hdd = hdd;
			So = so;
		}
	}
}
