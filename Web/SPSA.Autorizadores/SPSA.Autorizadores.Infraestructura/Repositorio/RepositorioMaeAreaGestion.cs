using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMaeAreaGestion : RepositorioGenerico<SGPContexto, Mae_AreaGestion>, IRepositorioMaeAreaGestion
    {
        public RepositorioMaeAreaGestion(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }
    }
}
