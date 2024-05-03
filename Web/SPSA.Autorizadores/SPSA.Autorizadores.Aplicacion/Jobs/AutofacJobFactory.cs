﻿using Quartz.Spi;
using Quartz;
using Autofac;

namespace SPSA.Autorizadores.Aplicacion.Jobs
{
	public class AutofacJobFactory : IJobFactory
	{
		private readonly IContainer _container;

		public AutofacJobFactory(IContainer container)
		{
			_container = container;
		}

		public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
		{
			return (IJob)_container.Resolve(bundle.JobDetail.JobType);
		}

		public void ReturnJob(IJob job)
		{
		}
	}
}
