using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class MonitorTrxInfo
    {
        public int ReturnValue { get; set; }
        public int NoCantTrx { get; set; }
        public decimal NoImpVta { get; set; }
        public int NoSqlCode { get; set; }
        public string VoError { get; set; }
    }
}
