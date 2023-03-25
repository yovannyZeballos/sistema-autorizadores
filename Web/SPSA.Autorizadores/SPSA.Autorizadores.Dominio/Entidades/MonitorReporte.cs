using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class MonitorReporte
    {
        public string CodEmpresa { get; private set; }
        public string CodLocal { get; private set; }
        public DateTime FechaProceso { get; private set; }
        public DateTime FechaCierre { get; private set; }
        public string HoraInicio { get; private set; }
        public string HoraFin { get; private set; }
        public string Estado { get; private set; }
        public string Observacion { get; private set; }
        public string Formato { get; private set; }

        public MonitorReporte(string codEmpresa, string codLocal, DateTime fechaProceso, DateTime fechaCierre, string horaInicio, string horaFin, string estado, string observacion, string formato)
        {
            CodEmpresa = codEmpresa;
            CodLocal = codLocal;
            FechaProceso = fechaProceso;
            FechaCierre = fechaCierre;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            Estado = estado;
            Observacion = observacion;
            Formato = formato;
        }
    }
}
