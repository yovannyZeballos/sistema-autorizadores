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
	public class ReporteDiferenciaCajaQuery : IRequest<ListarComunDTO<Dictionary<string, object>>>
	{
		public string CodigoEmpresa { get; set; }
		public string CodigoLocal { get; set; }
		public DateTime FechaInicio { get; set; }
		public DateTime FechaFin { get; set; }
	}

	public class ReporteDiferenciaCajaHandler : IRequestHandler<ReporteDiferenciaCajaQuery, ListarComunDTO<Dictionary<string, object>>>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public ReporteDiferenciaCajaHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}
		public async Task<ListarComunDTO<Dictionary<string, object>>> Handle(ReporteDiferenciaCajaQuery request, CancellationToken cancellationToken)

		{
			var response = new ListarComunDTO<Dictionary<string, object>> { Ok = true };

			try
			{
				response.Columnas = new List<string>();
				response.Data = new List<Dictionary<string, object>>();
				var dt = await _repositorioCajero.ReporteDiferenciaCajas(request.CodigoEmpresa, string.IsNullOrEmpty(request.CodigoLocal) ? "00" : request.CodigoLocal, request.FechaInicio, request.FechaFin);
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
