﻿using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Queries
{
	public class ListarCajeroVolanteQuery : IRequest<ListarComunDTO<Dictionary<string, object>>>
	{
		public string CodEmpresa { get; set; }
		public string CodCoordinador { get; set; }
	}

	public class ListarCajeroVolanteHandler : IRequestHandler<ListarCajeroVolanteQuery, ListarComunDTO<Dictionary<string, object>>>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public ListarCajeroVolanteHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}

		public async Task<ListarComunDTO<Dictionary<string, object>>> Handle(ListarCajeroVolanteQuery request, CancellationToken cancellationToken)
		{
			var response = new ListarComunDTO<Dictionary<string, object>>
			{
				Ok = true,
				Columnas = new List<string>(),
				Data = new List<Dictionary<string, object>>()
			};

			try
			{
				var dt = await _repositorioCajero.ListarCajeroVolante(request.CodEmpresa, request.CodCoordinador);

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
