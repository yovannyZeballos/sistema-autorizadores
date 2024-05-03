using MediatR;
using Quartz;
using Serilog;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Logger;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Jobs
{
	public class JobActualizacionEstadoCierre : IJob
	{
		private readonly IMediator _mediator;
		private readonly ILogger _logger;


		public JobActualizacionEstadoCierre(IMediator mediator)
		{
			_mediator = mediator;
			_logger = SerilogClass._log;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			_logger.Information("Job de actualización de estado de cierre iniciado !!");
			await _mediator.Send(new ProcesarActualizacionEstadoCierreCommand());
			_logger.Information("Job de actualización de estado de cierre finalizado !!");
		}
	}
}
