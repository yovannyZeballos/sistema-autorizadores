using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
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
            IQueryable<TEntidad> query = _contexto.Set<TEntidad>().AsNoTracking();
            return await query.AnyAsync(predicado);
        }

        public IQueryable<TEntidad> Obtener(Expression<Func<TEntidad, bool>> predicado = null)
        {
            IQueryable<TEntidad> query = _contexto.Set<TEntidad>();
            if (predicado != null) query = query.Where(predicado);
            return query;
        }

        public async Task<PagedResult<TEntidad>> ObtenerPaginado(
                        Expression<Func<TEntidad, bool>> predicado = null,
                        int pageNumber = 1,
                        int pageSize = 10,
                        Expression<Func<TEntidad, object>> orderBy = null,
                        bool ascending = true,
                        params Expression<Func<TEntidad, object>>[] includes)
        {
            // Obtiene la consulta base
            IQueryable<TEntidad> query = _contexto.Set<TEntidad>();

            // Aplicar Includes dinámicos
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Aplica el predicado si se ha definido
            if (predicado != null)
            {
                query = query.Where(predicado);
            }

            // Calcula el total de registros antes de aplicar Skip/Take
            int totalRecords = await query.CountAsync();

            // Aplica el ordenamiento si se ha definido
            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            // Aplica la paginación
            query = query.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

            // Ejecuta la consulta y obtiene la lista de registros
            List<TEntidad> items = await query.ToListAsync();

            // Devuelve la información paginada encapsulada en PagedResult
            return new PagedResult<TEntidad>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Items = items
            };
        }

        public void AgregarActualizar(TEntidad entity)
        {
            _contexto.Set<TEntidad>().AddOrUpdate(entity);
        }

        public void AgregarRango(List<TEntidad> entity)
        {
            _contexto.Set<TEntidad>().AddRange(entity);
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return await _contexto.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        // Método para descartar cambios en una entidad específica
        public void DescartarCambios(TEntidad entidad)
        {
            var entry = _contexto.Entry(entidad);
            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
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
