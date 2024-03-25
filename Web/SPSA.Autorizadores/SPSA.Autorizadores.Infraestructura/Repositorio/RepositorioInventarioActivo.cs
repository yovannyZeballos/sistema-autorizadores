using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioInventarioActivo : RepositorioGenerico<BCTContexto, Inv_Activo>, IRepositorioInventarioActivo
    {
        public RepositorioInventarioActivo(BCTContexto context) : base(context) { }

        public BCTContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }
    }
}
