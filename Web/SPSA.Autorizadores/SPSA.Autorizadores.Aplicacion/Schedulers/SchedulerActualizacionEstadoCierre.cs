using Autofac;
using Quartz;
using Quartz.Impl;
using Serilog;
using SPSA.Autorizadores.Aplicacion.Jobs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Schedulers
{
	public class SchedulerActualizacionEstadoCierre
	{
		public static void Start(IContainer container)
		{
			ILogger logger = SerilogClass._log;

			try
			{
				bool.TryParse(ConfigurationManager.AppSettings["EjecutarJobActualizacionEstadoCierre"], out bool ejecutarJobActualizacionEstadoCierre);

				if (!ejecutarJobActualizacionEstadoCierre)
				{
					logger.Information($"El Job de Actualizacion de Estado de Cierre no esta habilitado -> EjecutarJobActualizacionEstadoCierre = {ejecutarJobActualizacionEstadoCierre}");
					return;
				}

				IBCTContexto contexto = new BCTContexto();
				var proceso = contexto.RepositorioProceso.Obtener(x => x.CodProceso == Constantes.CodigoProcesoActualizacionEstadoCierre).FirstOrDefault();

				if (proceso == null)
				{
					logger.Error("No se encontro el proceso de Actualizacion de Estado de Cierre");
					return;
				}

				IScheduler scheduler = container.Resolve<IScheduler>();
				scheduler.JobFactory = new AutofacJobFactory(container);
				scheduler.Start().Wait();

				IJobDetail job = JobBuilder.Create<JobActualizacionEstadoCierre>().Build();

				ITrigger trigger = TriggerBuilder.Create()
					.WithCronSchedule(proceso.NomProceso)
					.Build();

				scheduler.ScheduleJob(job, trigger).Wait();
			}
			catch (System.Exception ex)
			{
				logger.Error(ex, "Error al iniciar el Scheduler de Actualizacion de Estado de Cierre");
			}
		}
	}
}
