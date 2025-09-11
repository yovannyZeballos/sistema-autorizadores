namespace SGP.Api.Models
{
    public class TicketGlpi
    {
        public string Id { get; set; }
        public DateTime FechaProceso { get; set; }
        public string Titulo { get; set; }
        public string Estado { get; set; }
        public string EncuestaSatisfaccionFechaCreacion { get; set; }
        public string Solicitante { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public string Categoria { get; set; }
        public string GrupoTecnicos { get; set; }
        public string Tecnico { get; set; }
        public string Localizaciones { get; set; }
        public string FuentesSolicitantes { get; set; }
        public string Prioridad { get; set; }
        public string Entidad { get; set; }
        public string FechaSolucion { get; set; }
        public string ElementosAsociados { get; set; }
        public string TiempoAdueñarse { get; set; }
        public string TiempoSolucion { get; set; }
        public string Autor { get; set; }
        public string TiempoSolucionEstadisticas { get; set; }
        public string TiempoAtenderServicio { get; set; }
        public string IncidentesSociedad1 { get; set; }
        public string SolicitudSociedad2 { get; set; }
        public string IncidentesAplicacionPrograma { get; set; }
        public string TiempoEspera { get; set; }
        public string TipoTarea { get; set; }
        public string GrupoSolicitante { get; set; }
    }
}
