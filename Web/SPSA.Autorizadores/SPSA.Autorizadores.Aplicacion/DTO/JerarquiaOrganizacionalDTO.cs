namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class JerarquiaOrganizacionalDTO : RespuestaComunDTO
	{
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
		public string NomEmpresa { get; set; }
		public string NomCadena { get; set; }
		public string NomRegion { get; set; }
		public string NomZona { get; set; }
		public string NomLocal { get; set; }

		public override string ToString()
		{
			return $"Empresa: {NomEmpresa} - Cadena: {NomCadena} - Region: {NomRegion} - Zona: {NomZona}";
		}
	}
}
