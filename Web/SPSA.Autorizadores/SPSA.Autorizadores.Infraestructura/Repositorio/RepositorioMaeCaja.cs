using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMaeCaja : RepositorioGenerico<SGPContexto, Mae_Caja>, IRepositorioMaeCaja
    {
        /// <summary>
		/// Inicializa una nueva instancia de la clase <see cref="RepositorioMaeCaja"/>.
		/// </summary>
		/// <param name="context">El contexto de la base de datos.</param>
		public RepositorioMaeCaja(SGPContexto context) : base(context) { }

        /// <summary>
        /// Obtiene el contexto de la base de datos.
        /// </summary>
        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }
    }
}
