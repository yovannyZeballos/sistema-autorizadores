using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajeros.Queries
{
	public class ListarCajerosQuery : IRequest<ListarCajerosDTO>
	{
		public string CodigoLocal { get; set; }
	}

	public class ListarCajerosHandler : IRequestHandler<ListarCajerosQuery, ListarCajerosDTO>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public ListarCajerosHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}
		public async Task<ListarCajerosDTO> Handle(ListarCajerosQuery request, CancellationToken cancellationToken)

		{
			var response = new ListarCajerosDTO { Ok = true };

			try
			{
				response.Columnas = new List<string>();
				response.Cajeros = new List<Dictionary<string, object>>();
				var dt = await _repositorioCajero.ListarCajero(request.CodigoLocal);
				foreach (DataColumn colum in dt.Columns)
				{
					response.Columnas.Add(colum.ColumnName);
				}

				response.Cajeros = dt.AsEnumerable()
						 .Select(r => r.Table.Columns.Cast<DataColumn>()
						 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
					  ).ToDictionary(z => z.Key.Replace(" ", "")
											  .Replace(".", "")
											  .Replace("á", "a")
											  .Replace("é", "e")
											  .Replace("í", "i")
											  .Replace("ó", "o")
											  .Replace("ú", "u"), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
				   ).ToList();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}
			return response;

		}
	}
}
