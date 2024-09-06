using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
	public class ListarProcesosQuery : IRequest<ListarComunDTO<ListarProcesoDTO>>
	{
	}

	public class ListarProcesosHandler : IRequestHandler<ListarProcesosQuery, ListarComunDTO<ListarProcesoDTO>>
	{
		private readonly ILogger _logger;

		public ListarProcesosHandler()
		{
			_logger = SerilogClass._log;
		}

		public async Task<ListarComunDTO<ListarProcesoDTO>> Handle(ListarProcesosQuery request, CancellationToken cancellationToken)
		{
			var respuesta = new ListarComunDTO<ListarProcesoDTO> { Ok = true };

			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					var procesos = await contexto.RepositorioProceso.Obtener(x => x.IndActivo == "A").ToListAsync(cancellationToken);
					respuesta.Data = procesos.Select(x => new ListarProcesoDTO
					{
						CodProceso = x.CodProceso,
						DesProceso = x.DesProceso,
					}).ToList();
				}

			}
			catch (Exception ex)
			{

				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
				_logger.Error(ex, respuesta.Mensaje);
			}
		
			return respuesta;
		}
	}
}
