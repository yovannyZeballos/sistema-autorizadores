using Npgsql;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Configuracion;
using SPSA.Autorizadores.Infraestructura.Repositorio;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Contexto
{
	[DbConfigurationType(typeof(NpgsqlConfiguration))]
	public class BCTContexto : DbContext, IBCTContexto
	{
		public DbSet<ProcesoParametro> ProcesoParametros { get; set; }

		public BCTContexto() : base("SGP")
		{
			RepositorioSegSistema = new RepositorioSegSistema(this);
			RepositorioProcesoParametroEmpresa = new RepositorioProcesoParametroEmpresa(this);
		}


		public IRepositorioSegSistema RepositorioSegSistema { get; private set; }
		public IRepositorioProcesoParametroEmpresa RepositorioProcesoParametroEmpresa { get; private set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new ProcesoParametroTypeConfiguration());
			modelBuilder.Configurations.Add(new ProcesoParametroEmpresaTypeConfiguration());
			modelBuilder.Configurations.Add(new SegSistemaTypeConfiguration());
		}

		public bool GuardarCambios()
		{
			return SaveChanges() > 0;
		}

		public async Task<bool> GuardarCambiosAsync()
		{
			var filasAfectadas = await SaveChangesAsync();
			return filasAfectadas > 0;
		}

		public void Reestablecer()
		{
			ChangeTracker
			.Entries()
			.ToList()
			.ForEach(x => x.Reload());
		}

		#region IDisposable Support
		public new void Dispose()
		{
			GC.SuppressFinalize(this);
		}
		#endregion
	}


	class NpgsqlConfiguration : DbConfiguration
	{
		public NpgsqlConfiguration()
		{
			SetProviderServices("Npgsql", NpgsqlServices.Instance);
			SetProviderFactory("Npgsql", NpgsqlFactory.Instance);
			SetDefaultConnectionFactory(new NpgsqlConnectionFactory());
		}
	}
}
