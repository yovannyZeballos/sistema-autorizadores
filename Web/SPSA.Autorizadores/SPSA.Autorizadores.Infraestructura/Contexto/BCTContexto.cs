using Npgsql;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Configuracion;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Contexto
{
	[DbConfigurationType(typeof(NpgsqlConfiguration))]
	public class BCTContexto : DbContext
	{
		static string ConnectionString = "";
		static NpgsqlConnection cn = new NpgsqlConnection("Host=172.31.23.205; Database=SGP; Username=sgp_dev; Password=',I;bIvR&lt;x5rN8&quot;7I'");
		public DbSet<ProcesoParametro> ProcesoParametros { get; set; }

		public BCTContexto() : base("SGP")
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new ProcesoParametroTypeConfiguration());
		}
	}


	class NpgsqlConfiguration :DbConfiguration
	{
		public NpgsqlConfiguration()
		{
			SetProviderServices("Npgsql", NpgsqlServices.Instance);
			SetProviderFactory("Npgsql", NpgsqlFactory.Instance);
			SetDefaultConnectionFactory(new NpgsqlConnectionFactory());
		}
	}
}
