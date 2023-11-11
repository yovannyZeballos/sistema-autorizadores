using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{
	public class ReporteCierreResumenQuery : IRequest<ListarComunDTO<Dictionary<string, object>>>
	{
		public string CodSociedad { get; set; }
		public short Año { get; set; }
		public short Mes { get; set; }
		public int Opcion { get; set; }
	}

	public class ReporteCierreResumenHandler : IRequestHandler<ReporteCierreResumenQuery, ListarComunDTO<Dictionary<string, object>>>
	{
		private readonly IRepositorioCajaCierre _repositorioCajaCierre;

		public ReporteCierreResumenHandler(IRepositorioCajaCierre repositorioCajaCierre)
		{
			_repositorioCajaCierre = repositorioCajaCierre;
		}

		public async Task<ListarComunDTO<Dictionary<string, object>>> Handle(ReporteCierreResumenQuery request, CancellationToken cancellationToken)
		{
			var response = new ListarComunDTO<Dictionary<string, object>>
			{
				Ok = true,
				Columnas = new List<string>(),
				Data = new List<Dictionary<string, object>>()
			};

			try
			{
				var fechaInicio = new DateTime(request.Año, request.Mes, 1);
				var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
				var dt = await _repositorioCajaCierre.ListarResumen(request.CodSociedad, request.Opcion, fechaInicio, fechaFin);

				foreach (DataColumn colum in dt.Columns)
				{
					response.Columnas.Add(colum.ColumnName.Replace(" ", "")
												  .Replace(".", "")
												  .Replace("á", "a")
												  .Replace("é", "e")
												  .Replace("í", "i")
												  .Replace("ó", "o")
												  .Replace("ú", "u"));

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
