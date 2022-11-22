using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Colaborador : Persona
    {
        public string FechaIngreso { get; set; }
        public string CodigoLocal { get; set; }
        public string DescLocal { get; set; }
        public string CodigoPuesto { get; set; }
        public string DescPuesto { get; set; }
        public string Planilla { get; set; }
    }
}
