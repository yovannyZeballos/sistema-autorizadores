namespace SGP.Api.Models
{
    public class ProcesoParamPorEmpresa
    {
        public int CodProceso { get; set; }
        public string? CodEmpresa { get; set; } = string.Empty;
        public string? CodParametro { get; set; } = string.Empty;
        public string? NomParametro { get; set; } = string.Empty;
        public string? ValParametro { get; set; } = string.Empty;
    }
}
