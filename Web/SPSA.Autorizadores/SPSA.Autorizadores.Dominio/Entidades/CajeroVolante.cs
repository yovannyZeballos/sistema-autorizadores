namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class CajeroVolante
	{
		public string CodOfisis { get; private set; }
		public string NumDocumento { get; private set; }
		public string CodEmpresaOrigen { get; private set; }
		public decimal CodSedeOrigen { get; private set; }
		public string CodEmpresa { get; private set; }
		public decimal CodSede { get; private set; }
		public string Coordinador { get; private set; }
		public string Usuario { get; private set; }

		public CajeroVolante(string codOfisis, string numDocumento, string codEmpresaOrigen, decimal codSedeOrigen, string codEmpresa, decimal codSede, string coordinador, string usuario)
		{
			CodOfisis = codOfisis;
			NumDocumento = numDocumento;
			CodEmpresaOrigen = codEmpresaOrigen;
			CodSedeOrigen = codSedeOrigen;
			CodEmpresa = codEmpresa;
			CodSede = codSede;
			Coordinador = coordinador;
			Usuario = usuario;
		}

		public CajeroVolante(string codOfisis, string codEmpresa, decimal codSede, string usuario)
		{
			CodOfisis = codOfisis;
			CodEmpresa = codEmpresa;
			CodSede = codSede;
			Usuario = usuario;
		}
	}
}
