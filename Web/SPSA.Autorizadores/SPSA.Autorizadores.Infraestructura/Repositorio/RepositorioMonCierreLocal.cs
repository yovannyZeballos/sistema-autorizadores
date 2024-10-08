﻿using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioMonCierreLocal : RepositorioGenerico<SGPContexto, MonCierreLocal>, IRepositorioMonCierreLocal
	{
		public RepositorioMonCierreLocal(SGPContexto context) : base(context) { }

		public SGPContexto BCTContexto
		{
			get { return _contexto; }
		}

		public async Task InsertarActualizar()
		{
			await _contexto.Database.ExecuteSqlCommandAsync("CALL \"SGP\".\"SP_INSERTAR_ACTUALIZAR_MON_CIERRE_LOCAL\"()");
		}
	}
}
