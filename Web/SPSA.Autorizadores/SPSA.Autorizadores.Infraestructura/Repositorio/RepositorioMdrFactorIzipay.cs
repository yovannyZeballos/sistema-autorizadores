using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMdrFactorIzipay : RepositorioGenerico<SGPContexto, Mdr_FactorIzipay>, IRepositorioMdrFactorIzipay
    {
        public RepositorioMdrFactorIzipay(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }
    }
}
