using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries
{
	public class ListarInventarioServidorQuery : IRequest<ListarInventarioServidorDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
	}

	public class ListarInventarioServidorHandler : IRequestHandler<ListarInventarioServidorQuery, ListarInventarioServidorDTO>
	{
		private readonly IRepositorioInventarioServidor _repositorioInventarioServidor;

		public ListarInventarioServidorHandler(IRepositorioInventarioServidor repositorioInventarioServidor)
		{
			_repositorioInventarioServidor = repositorioInventarioServidor;
		}

		public async Task<ListarInventarioServidorDTO> Handle(ListarInventarioServidorQuery request, CancellationToken cancellationToken)
		{
			var servidores = new ListarInventarioServidorDTO { Ok = true };

			try
			{
				var servidoresDataTable = await _repositorioInventarioServidor.Listar(request.CodEmpresa, request.CodFormato, request.CodLocal);

				servidores.Columnas = new List<string>();
				foreach (DataColumn colum in servidoresDataTable.Columns)
				{
					servidores.Columnas.Add(colum.ColumnName);
				}

				servidores.Servidores = servidoresDataTable.AsEnumerable()
						 .Select(r => r.Table.Columns.Cast<DataColumn>()
						 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
					  ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
				   ).ToList();
			}
			catch (Exception ex)
			{
				servidores.Ok = false;
				servidores.Mensaje = ex.Message;
			}

			return servidores;
		}
	}
}
