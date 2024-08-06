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

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{
	public class ListarMotivosReimpresionQuery : IRequest<GenericResponseDTO<Dictionary<string, string>>>
	{
	}

	public class ListarMotivosReimpresionHandler : IRequestHandler<ListarMotivosReimpresionQuery, GenericResponseDTO<Dictionary<string, string>>>
	{
		private readonly ILogger _logger;

		public ListarMotivosReimpresionHandler()
		{
			_logger = SerilogClass._log;
		}

		public async Task<GenericResponseDTO<Dictionary<string, string>>> Handle(ListarMotivosReimpresionQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<Dictionary<string, string>> { Ok = true };

			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					var parametros = await contexto.RepositorioProcesoParametro.Obtener(x => x.CodProceso == Constantes.CodigoProcesoImpresionCodigoBarras && x.IndActivo == "S").ToListAsync();
					response.Data = parametros.ToDictionary(x => x.CodParametro, x => x.ValParametro);
				}
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Error al listar motivos de reimpresion";
				_logger.Error(ex, response.Mensaje);

			}

			return response;
		}
	}
}
