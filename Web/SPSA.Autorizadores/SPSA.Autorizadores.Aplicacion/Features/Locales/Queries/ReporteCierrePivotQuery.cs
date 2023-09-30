using MediatR;
using NReco.PivotData;
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
	public class ReporteCierrePivotQuery : IRequest<ListarComunDTO<Dictionary<string, object>>>
	{
		public string CodEmpresa { get; set; }
		public short Año { get; set; }
		public short Mes { get; set; }
	}

	public class ReporteCierrePivotHandler : IRequestHandler<ReporteCierrePivotQuery, ListarComunDTO<Dictionary<string, object>>>
	{
		private readonly IRepositorioCajaCierre _repositorioCajaCierre;

		public ReporteCierrePivotHandler(IRepositorioCajaCierre repositorioCajaCierre)
		{
			_repositorioCajaCierre = repositorioCajaCierre;
		}

		public async Task<ListarComunDTO<Dictionary<string, object>>> Handle(ReporteCierrePivotQuery request, CancellationToken cancellationToken)
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
				var dt = await _repositorioCajaCierre.Listar(request.CodEmpresa, fechaInicio, fechaFin);

				var pivotData = new PivotData(
					new string[] { "LOC_DESCRIPCION", "CIE_FCONTABLE_FORMAT" },
					new MaxAggregatorFactory("CIE_ESTADO"),
					new DataTableReader(dt));

				var pvtTbl = new PivotTable(
					new[] { "LOC_DESCRIPCION" },
					new[] { "CIE_FCONTABLE_FORMAT" },
					pivotData);

				var dtResult = new DataTable();
				dtResult.Columns.Add("Local");
				foreach (var column in pvtTbl.ColumnKeys)
				{
					dtResult.Columns.Add(column.DimKeys[0].ToString());
				}

				for (int i = 0; i < pvtTbl.RowKeys.Count(); i++)
				{
					DataRow dr = dtResult.NewRow();
					dr["Local"] = pvtTbl.RowKeys[i].DimKeys[0].ToString();
					for (int j = 0; j < pvtTbl.ColumnKeys.Count(); j++)
					{
						dr[pvtTbl.ColumnKeys[j].DimKeys[0].ToString()] = pvtTbl[i, j].Value;
					}

					dtResult.Rows.Add(dr);
				}

				foreach (DataColumn colum in dtResult.Columns)
				{
					response.Columnas.Add(colum.ColumnName);

				}

				response.Data = dtResult.AsEnumerable()
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
