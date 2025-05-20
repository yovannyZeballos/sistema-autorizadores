using System;
using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioCComSolicitudDet : RepositorioGenerico<SGPContexto, CCom_SolicitudDet>, IRepositorioCComSolicitudDet
    {
        public RepositorioCComSolicitudDet(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }

        public Task<DataTable> ObtenerSolicitudDetPorEmpresaAsync(string codEmpresa)
        {
            throw new NotImplementedException();
        }
    }
}
