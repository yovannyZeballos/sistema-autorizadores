using System;
using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMaeColaboradorExt : RepositorioGenerico<SGPContexto, Mae_ColaboradorExt>, IRepositorioMaeColaboradorExt
    {
        public RepositorioMaeColaboradorExt(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }

        public Task<DataTable> ObtenerColaboradoresPorEmpresaAsync(string codEmpresa)
        {
            throw new NotImplementedException();
        }
    }
}
