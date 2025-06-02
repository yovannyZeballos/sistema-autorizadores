using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMdrBinesIzipay : RepositorioGenerico<SGPContexto, Mdr_BinesIzipay>, IRepositorioMdrBinesIzipay
    {
        public RepositorioMdrBinesIzipay(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }
    }
}
