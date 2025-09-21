using MediatR;
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

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Search { get; set; }
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

                // columnas dinámicas
                foreach (DataColumn col in dt.Columns)
                {
                    response.Columnas.Add(col.ColumnName.Replace(" ", "")
                                                        .Replace(".", "")
                                                        .Replace("á", "a")
                                                        .Replace("é", "e")
                                                        .Replace("í", "i")
                                                        .Replace("ó", "o")
                                                        .Replace("ú", "u"));
                }

                // total sin filtros
                response.TotalRegistros = dt.Rows.Count;

                IEnumerable<DataRow> query = dt.AsEnumerable();

                // búsqueda
                if (!string.IsNullOrEmpty(request.Search))
                {
                    query = query.Where(r => r.ItemArray.Any(c => c.ToString().Contains(request.Search)));
                }

                response.TotalFiltrados = query.Count();

                // paginación
                query = query.Skip((request.PageNumber - 1) * request.PageSize)
                             .Take(request.PageSize);


                // filas dinámicas
                response.Data = query
                    .Select(r => r.Table.Columns.Cast<DataColumn>()
                    .ToDictionary(
                        c => c.ColumnName.Replace(" ", "")
                                         .Replace(".", "")
                                         .Replace("á", "a")
                                         .Replace("é", "e")
                                         .Replace("í", "i")
                                         .Replace("ó", "o")
                                         .Replace("ú", "u"),
                        c => r[c.Ordinal] is DateTime d ? d.ToString("dd/MM/yyyy") : r[c.Ordinal]
                    ))
                    .ToList();

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
