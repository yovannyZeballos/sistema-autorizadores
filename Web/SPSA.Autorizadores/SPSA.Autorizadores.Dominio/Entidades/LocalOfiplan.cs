
namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class LocalOfiplan
    {
        public string CodCadenaCt2 { get; private set; }
        public string CodLocalCt2 { get; private set; }
        public string CodEmpresa { get; private set; }
        public string CodSede { get; private set; }

        public LocalOfiplan(string codCadenaCt2, string codLocalCt2, string codEmpresa, string codSede)
        {
            CodCadenaCt2 = codCadenaCt2;
            CodLocalCt2 = codLocalCt2;
            CodEmpresa = codEmpresa;
            CodSede = codSede;
        }
    }
}
