using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
	public class ListarLocalMonitorQuery : IRequest<ListarComunDTO<Dictionary<string, object>>>
	{
		public string CodEmpresa { get; set; }
		public string Fecha { get; set; }
		public string Estado { get; set; }
		public int Tipo { get; set; }
	}

	public class ListarLocalMonitorHandler : IRequestHandler<ListarLocalMonitorQuery, ListarComunDTO<Dictionary<string, object>>>
	{
		private readonly IRepositorioMonitorReporte _repositorioLocalMonitor;

		public ListarLocalMonitorHandler(IRepositorioMonitorReporte repositorioLocalMonitor)
		{
			_repositorioLocalMonitor = repositorioLocalMonitor;
		}

		public async Task<ListarComunDTO<Dictionary<string, object>>> Handle(ListarLocalMonitorQuery request, CancellationToken cancellationToken)
		{
			var response = new ListarComunDTO<Dictionary<string, object>> { Ok = true };

			try
			{
				var fechaValida = DateTime.TryParseExact(request.Fecha, "dd/MM/yyyy", new CultureInfo("es-PE"), DateTimeStyles.None, out DateTime fecha);

				if (!fechaValida)
				{
					response.Ok = false;
					response.Mensaje = "El formato de la fecha ingresada es invalida";
					return response;
				}


				response.Columnas = new List<string>();
				response.Data = new List<Dictionary<string, object>>();
				var dt = await _repositorioLocalMonitor.ListarMonitorReporte(request.CodEmpresa, fecha, request.Estado, request.Tipo);
				foreach (DataColumn colum in dt.Columns)
				{
					response.Columnas.Add(colum.ColumnName);
				}

				response.Data = dt.AsEnumerable()
						.Select(r => r.Table.Columns.Cast<DataColumn>()
						.Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
					 ).ToDictionary(z => z.Key.Replace(" ", "")
											  .Replace(".", "")
											  .Replace("á", "a")
											  .Replace("é", "e")
											  .Replace("í", "i")
											  .Replace("ó", "o")
											  .Replace("ú", "u"), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy HH:mm:ss") : z.Value)
				  ).ToList();

				if (response.Data.Count == 0)
				{
					response.Ok = false;
					response.Mensaje = "No se encuentra información de cierre sobre la fecha ingresada.";
					return response;
				}
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
