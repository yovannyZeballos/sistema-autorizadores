using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMaeMarca : RepositorioGenerico<SGPContexto, Mae_Marca>, IRepositorioMaeMarca
    {
        public RepositorioMaeMarca(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }
    }
}
