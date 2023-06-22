using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries
{
	public class ListarCajaInventarioQuery : IRequest<ListarCajaInventarioDTO>
	{
	}

	public class ListarCajaInventarioHandler : IRequestHandler<ListarCajaInventarioQuery, ListarCajaInventarioDTO>
	{
		private readonly IRepositorioSovosInventarioCaja _repositorioSovosInventarioCaja;

		public ListarCajaInventarioHandler(IRepositorioSovosInventarioCaja repositorioSovosInventarioCaja)
		{
			_repositorioSovosInventarioCaja = repositorioSovosInventarioCaja;
		}

		public async Task<ListarCajaInventarioDTO> Handle(ListarCajaInventarioQuery request, CancellationToken cancellationToken)
		{
			var cajas = new ListarCajaInventarioDTO { Ok = true };

			try
			{
				var localesDataTable = await _repositorioSovosInventarioCaja.Listar();

				cajas.Columnas = new List<string>();
				foreach (DataColumn colum in localesDataTable.Columns)
				{
					cajas.Columnas.Add(colum.ColumnName);
				}

				cajas.Cajas = localesDataTable.AsEnumerable()
						 .Select(r => r.Table.Columns.Cast<DataColumn>()
						 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
					  ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
				   ).ToList();
			}
			catch (Exception ex)
			{
				cajas.Ok = false;
				cajas.Mensaje = ex.Message;
				throw;
			}

			return cajas;
		}
	}
}
