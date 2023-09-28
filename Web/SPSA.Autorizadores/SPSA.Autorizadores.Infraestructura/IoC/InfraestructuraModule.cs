using Autofac;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Repositorio;
using SPSA.Autorizadores.Infraestructura.Utiles;

namespace SPSA.Autorizadores.Infraestructura.IoC
{
	public class InfraestructuraModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<RepositorioAutorizadores>().As<IRepositorioAutorizadores>().InstancePerRequest();
			builder.RegisterType<RepositorioEmpresa>().As<IRepositorioEmpresa>().InstancePerRequest();
			builder.RegisterType<RepositorioLocal>().As<IRepositorioLocal>().InstancePerRequest();
			builder.RegisterType<RepositorioSeguridad>().As<IRepositorioSeguridad>().InstancePerRequest();
			builder.RegisterType<RepositorioPuesto>().As<IRepositorioPuesto>().InstancePerRequest();
			builder.RegisterType<RepositorioMonitorReporte>().As<IRepositorioMonitorReporte>().InstancePerRequest();
			builder.RegisterType<RepositorioSovosLocal>().As<IRepositorioSovosLocal>().InstancePerRequest();
			builder.RegisterType<RepositorioSovosFormato>().As<IRepositorioSovosFormato>().InstancePerRequest();
			builder.RegisterType<DBHelper>().InstancePerRequest();
			builder.RegisterType<RepositorioSovosCaja>().As<IRepositorioSovosCaja>().InstancePerRequest();
			builder.RegisterType<RepositorioSovosInventarioCaja>().As<IRepositorioSovosInventarioCaja>().InstancePerRequest();
			builder.RegisterType<RepositorioLocalOfiplan>().As<IRepositorioLocalOfiplan>().InstancePerRequest();
			builder.RegisterType<RepositorioInventarioTipo>().As<IRepositorioInventarioTipo>().InstancePerRequest();
			builder.RegisterType<RepositorioInventarioServidor>().As<IRepositorioInventarioServidor>().InstancePerRequest();
			builder.RegisterType<RepositorioInventarioServidorVirtual>().As<IRepositorioInventarioServidorVirtual>().InstancePerRequest();
			builder.RegisterType<RepositorioCajero>().As<IRepositorioCajero>().InstancePerRequest();
			builder.RegisterType<RepositorioProcesos>().As<IRepositorioProcesos>().InstancePerRequest();
			base.Load(builder);
		}
	}
}
