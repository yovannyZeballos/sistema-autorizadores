using Autofac;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            base.Load(builder);
        }
    }
}
