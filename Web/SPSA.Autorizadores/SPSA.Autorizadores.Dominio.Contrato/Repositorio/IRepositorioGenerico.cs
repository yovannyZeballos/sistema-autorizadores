﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	/// <summary>
	/// Define los métodos para un repositorio genérico.
	/// </summary>
	/// <typeparam name="TEntidad">El tipo de entidad que el repositorio manejará.</typeparam>
	public interface IRepositorioGenerico<TEntidad> : IDisposable
		where TEntidad : class
	{
		/// <summary>
		/// Agrega una entidad al repositorio.
		/// </summary>
		/// <param name="entity">La entidad a agregar.</param>
		void Agregar(TEntidad entity);

		/// <summary>
		/// Agrega una entidad al repositorio.
		/// </summary>
		/// <param name="entity">La entidad a agregar.</param>
		void AgregarRango(List<TEntidad> entity);

		/// <summary>
		/// Actualiza una entidad en el repositorio.
		/// </summary>
		/// <param name="entity">La entidad a actualizar.</param>
		void Actualizar(TEntidad entity);

		/// <summary>
		/// Elimina una entidad del repositorio.
		/// </summary>
		/// <param name="entity">La entidad a eliminar.</param>
		void Eliminar(TEntidad entity);

		/// <summary>
		/// Comprueba si existe una entidad que cumple con el predicado especificado.
		/// </summary>
		/// <param name="predicado">El predicado para comprobar.</param>
		/// <returns>Un valor de tarea que indica si existe una entidad que cumple con el predicado especificado.</returns>
		Task<bool> Existe(Expression<Func<TEntidad, bool>> predicado);

		/// <summary>
		/// Obtiene las entidades que cumplen con el predicado especificado.
		/// </summary>
		/// <param name="predicado">El predicado para filtrar las entidades. Si es null, se devolverán todas las entidades.</param>
		/// <returns>Las entidades que cumplen con el predicado especificado.</returns>
		IQueryable<TEntidad> Obtener(Expression<Func<TEntidad, bool>> predicado = null);

		/// <summary>
		/// Elimina un rango de entidades del repositorio.
		/// </summary>
		/// <param name="entity">Las entidades a eliminar.</param>
		void EliminarRango(ICollection<TEntidad> entities);

		/// <summary>
		/// Agrega una entidad al repositorio.
		/// </summary>
		/// <param name="entity">La entidad a agregar.</param>
		void AgregarActualizar(TEntidad entity);

		Task<PagedResult<TEntidad>> ObtenerPaginado(
						Expression<Func<TEntidad, bool>> predicado = null,
						int pageNumber = 1,
						int pageSize = 10,
						Expression<Func<TEntidad, object>> orderBy = null,
						bool ascending = true,
                        params Expression<Func<TEntidad, object>>[] includes);

        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

		void DescartarCambios(TEntidad entidad);
    }
}
