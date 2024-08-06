using AutoMapper;
using AutoMapper.Mappers;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Entities;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{

	public class ListarAutorizadoresQuery : IRequest<ListarAutorizadorDTO>
	{
		public string CodigoLocal { get; set; } = string.Empty;
	}

	public class ListarAutorizadoresHandler : IRequestHandler<ListarAutorizadoresQuery, ListarAutorizadorDTO>
	{
		private readonly IMapper _mapper;
		private readonly IRepositorioAutorizadores _repositorioAutorizadores;

		public ListarAutorizadoresHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa, IRepositorioAutorizadores repositorioAutorizadores)
		{
			_mapper = mapper;
			_repositorioAutorizadores = repositorioAutorizadores;
		}

		public async Task<ListarAutorizadorDTO> Handle(ListarAutorizadoresQuery request, CancellationToken cancellationToken)
		{
			var respuesta = new ListarAutorizadorDTO();

			var autorizadoresDataTable = await _repositorioAutorizadores.ListarAutorizador(request.CodigoLocal);

			try
			{
				respuesta.Columnas = new List<string>();
				foreach (DataColumn colum in autorizadoresDataTable.Columns)
				{
					respuesta.Columnas.Add(colum.ColumnName);
				}

				respuesta.Columnas.Add("Impresion");

				var lst = autorizadoresDataTable.AsEnumerable()
						 .Select(r => r.Table.Columns.Cast<DataColumn>()
						 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
					  ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
				   ).ToList();

				var impresiones = await ObtenerImpresiones(request.CodigoLocal);

				lst = lst.Select(autorizador =>
				{
					var impresion = impresiones.FirstOrDefault(x => x.CodAutorizador == autorizador["Autorizador"].ToString());
					autorizador["Impresion"] = impresion?.Correlativo ?? 0;
					return autorizador;
				}).ToList();

				respuesta.Ok = true;
				respuesta.Autorizadores = lst;
			}
			catch (System.Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return respuesta;
		}

		private async Task<List<AutImpresion>> ObtenerImpresiones(string codLocal)
		{
			using (ISGPContexto contexto = new SGPContexto())
			{
				return await contexto.RepositorioAutImpresion.Obtener(x => x.CodLocal == codLocal).ToListAsync();
			}
		}
	}
}
