
namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Puesto
    {
        public string CodEmpresa { get; private set; }
        public string CodPuesto { get; private set; }
        public string Nombre { get; private set; }
        public string Select { get; private set; }

        public Puesto(string codEmpresa, string codPuesto, string select)
        {
            CodEmpresa = codEmpresa;
            CodPuesto = codPuesto;
            Select = select;
        }
    }
}
