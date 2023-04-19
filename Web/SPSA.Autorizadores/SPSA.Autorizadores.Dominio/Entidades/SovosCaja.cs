
namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class SovosCaja
    {
        public string CodEmpresa { get; private set; }
        public string CodLocal { get; private set; }
        public string CodFormato { get; private set; }
        public decimal NumeroCaja { get; private set; }
        public string Ip { get; private set; }
        public string So { get; private set; }

        public SovosCaja(string codEmpresa, string codLocal, string codFormato, decimal numeroCaja, string ip, string so)
        {
            CodEmpresa = codEmpresa;
            CodLocal = codLocal;
            CodFormato = codFormato;
            NumeroCaja = numeroCaja;
            Ip = ip;
            So = so;
        }
    }
}
