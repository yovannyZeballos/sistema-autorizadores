using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries
{
	/// <summary>
	/// Query para listar las empresas asociadas a un proceso.
	/// </summary>
	public class ListarEmpresasPorProcesoQuery : IRequest<GenericResponseDTO<List<ListarEmpresaDTO>>>
	{
		public decimal CodProceso { get; set; }
	}

	/// <summary>
	/// Manejador para el query ListarEmpresasPorProcesoQuery.
	/// </summary>
	public class ListarEmpresasPorProcesoHandler : IRequestHandler<ListarEmpresasPorProcesoQuery, GenericResponseDTO<List<ListarEmpresaDTO>>>
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para el manejador ListarEmpresasPorProcesoHandler.
		/// </summary>
		public ListarEmpresasPorProcesoHandler()
		{
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el query ListarEmpresasPorProcesoQuery.
		/// </summary>
		public async Task<GenericResponseDTO<List<ListarEmpresaDTO>>> Handle(ListarEmpresasPorProcesoQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarEmpresaDTO>> { Ok = true, Data = new List<ListarEmpresaDTO>() };

			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					var procesoEmpresas = await contexto.RepositorioProcesoEmpresa.Obtener(x => x.CodProceso == request.CodProceso && x.IndActivo == "S")
						.ToListAsync();
					var empresas = await contexto.RepositorioMaeEmpresa.Obtener().ToListAsync();

					foreach (var procesoEmpresa in procesoEmpresas)
					{
						var empresa = empresas.FirstOrDefault(x => x.CodEmpresa == procesoEmpresa.CodEmpresa);
						if (empresa != null)
						{
							response.Data.Add(new ListarEmpresaDTO
							{
								CodEmpresa = empresa.CodEmpresa,
								NomEmpresa = empresa.NomEmpresa,
								Ruc = empresa.Ruc
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las mepresas asociadas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}