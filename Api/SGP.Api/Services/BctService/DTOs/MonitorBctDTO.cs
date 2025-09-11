namespace SGP.Api.Services.BctService.DTOs
{
    public class MonitorBctDTO
    {
        public string? CodEmpresa { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public DateTime Fecha { get; set; }
        public int Hora { get; set; } = 0;
        public int Minuto { get; set; } = 0;
        public int Cantidad { get; set; } = 0;
        public int Limite { get; set; } = 0;
    }
}
