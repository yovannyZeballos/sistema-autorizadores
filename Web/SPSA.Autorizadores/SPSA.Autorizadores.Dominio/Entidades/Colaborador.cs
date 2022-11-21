using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Colaborador
    {
        public string Codigo { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public string FechaIngreso { get; set; }
        public string Estado { get; set; }
        public string CodigoLocal { get; set; }
        public string DescLocal { get; set; }
        public string CodigoPuesto { get; set; }
        public string DescPuesto { get; set; }
        public string Planilla { get; set; }
        public string NumeroDocumento { get; set; }
    }
}
