namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class Cajero
	{
		public string CodLocal { get; private set; }
		public string Nombres { get; private set; }
		public string Apeliidos { get; private set; }
		public string Tipo { get; private set; }
		public string TipoContrato { get; private set; }
		public string Rut { get; private set; }
		public string TipoDocIdentidad { get; private set; }
		public string CodigoEmpleado { get; private set; }
		public string Usuario { get; private set; }

		public Cajero(string codLocal, string nombres, string apeliidos, string tipo, string tipoContrato, string rut, string tipoDocIdentidad, string codigoEmpleado, string usuario)
		{
			CodLocal = codLocal;
			Nombres = nombres;
			Apeliidos = apeliidos;
			Tipo = tipo;
			TipoContrato = tipoContrato;
			Rut = rut;
			TipoDocIdentidad = tipoDocIdentidad;
			CodigoEmpleado = codigoEmpleado;
			Usuario = usuario;
		}
	}
}
