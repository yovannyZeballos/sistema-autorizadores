using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries
{
	public class ListarLocalesPorEmpresaQuery : IRequest<ListarLocalesPorEmpresaDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
	}

	public class ListarLocalesPorEmpresaHandler : IRequestHandler<ListarLocalesPorEmpresaQuery, ListarLocalesPorEmpresaDTO>
	{
		private readonly IRepositorioSovosLocal _repositorioSovosLocal;

		public ListarLocalesPorEmpresaHandler(IRepositorioSovosLocal repositorioSovosLocal)
		{
			_repositorioSovosLocal = repositorioSovosLocal;
		}

		public async Task<ListarLocalesPorEmpresaDTO> Handle(ListarLocalesPorEmpresaQuery request, CancellationToken cancellationToken)
		{
			var locales = new ListarLocalesPorEmpresaDTO { Ok = true };

			try
			{
				var localesDataTable = await _repositorioSovosLocal.ListarPorEmpresa(request.CodEmpresa, request.CodFormato);

				locales.Columnas = new List<string>();
				foreach (DataColumn colum in localesDataTable.Columns)
				{
					locales.Columnas.Add(colum.ColumnName);
				}

				locales.Locales = localesDataTable.AsEnumerable()
						 .Select(r => r.Table.Columns.Cast<DataColumn>()
						 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
					  ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
				   ).ToList();
			}
			catch (Exception ex)
			{
				locales.Ok = false;
				locales.Mensaje = ex.Message;
				throw;
			}

			return locales;
		}
	}
}
