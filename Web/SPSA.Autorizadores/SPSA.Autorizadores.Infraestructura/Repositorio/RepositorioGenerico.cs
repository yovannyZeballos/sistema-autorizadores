using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioGenerico<TContexto, TEntidad> : IRepositorioGenerico<TEntidad>
		 where TEntidad : class
		 where TContexto : DbContext
	{
		protected readonly TContexto _contexto;

		protected RepositorioGenerico(TContexto context)
		{
			_contexto = context;
		}

		public void Agregar(TEntidad entity)
		{
			_contexto.Set<TEntidad>().Add(entity);
		}

		public void Actualizar(TEntidad entidad)
		{
			_contexto.Entry(entidad).State = EntityState.Modified;
		}

		public void Eliminar(TEntidad entidad)
		{
			_contexto.Set<TEntidad>().Remove(entidad);
		}

		public void EliminarRango(ICollection<TEntidad> entidades)
		{
			_contexto.Set<TEntidad>().RemoveRange(entidades);
		}

		public async Task<bool> Existe(Expression<Func<TEntidad, bool>> predicado)
		{
			IQueryable<TEntidad> query = _contexto.Set<TEntidad>();
			return await query.AnyAsync(predicado);
		}

		public IQueryable<TEntidad> Obtener(Expression<Func<TEntidad, bool>> predicado = null)
		{
			IQueryable<TEntidad> query = _contexto.Set<TEntidad>();
			if (predicado != null) query = query.Where(predicado);
			return query;
		}
		#region IDisposable Support
		private bool disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_contexto.Dispose();
				}

				disposedValue = true;
			}
		}

		RepositorioGenerico()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		#endregion
	}
}
