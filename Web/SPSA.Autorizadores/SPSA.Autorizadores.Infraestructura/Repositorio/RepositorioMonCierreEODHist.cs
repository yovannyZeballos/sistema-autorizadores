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
	public class RepositorioMonCierreEODHist : RepositorioGenerico<SGPContexto, MonCierreEODHist>, IRepositorioMonCierreEODHist
	{
		public RepositorioMonCierreEODHist(SGPContexto context) : base(context) { }

		public SGPContexto SGPContext
		{
			get { return _contexto; }
		}
	}
}
