using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries
{
	public class ListarInventarioServidorVirtualQuery : IRequest<ListarInventarioServidorVirtualDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
		public string NumeroServidor { get; set; }
	}

	public class ListarInventarioServidorVirtualHandler : IRequestHandler<ListarInventarioServidorVirtualQuery, ListarInventarioServidorVirtualDTO>
	{
		private readonly IRepositorioInventarioServidorVirtual _repositorioInventarioServidorVirtual;

		public ListarInventarioServidorVirtualHandler(IRepositorioInventarioServidorVirtual repositorioInventarioServidorVirtual)
		{
			_repositorioInventarioServidorVirtual = repositorioInventarioServidorVirtual;
		}

		public async Task<ListarInventarioServidorVirtualDTO> Handle(ListarInventarioServidorVirtualQuery request, CancellationToken cancellationToken)
		{
			var virtuales = new ListarInventarioServidorVirtualDTO { Ok = true };

			try
			{
				var virtualesDataTable = await _repositorioInventarioServidorVirtual.Listar(request.CodEmpresa, request.CodFormato, request.CodLocal, request.NumeroServidor);

				virtuales.Columnas = new List<string>();
				foreach (DataColumn colum in virtualesDataTable.Columns)
				{
					virtuales.Columnas.Add(colum.ColumnName);
				}

				virtuales.Virtuales = virtualesDataTable.AsEnumerable()
						 .Select(r => r.Table.Columns.Cast<DataColumn>()
						 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
					  ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy HH:mm:ss") : z.Value)
				   ).ToList();
			}
			catch (Exception ex)
			{
				virtuales.Ok = false;
				virtuales.Mensaje = ex.Message;
			}

			return virtuales;
		}
	}
}
