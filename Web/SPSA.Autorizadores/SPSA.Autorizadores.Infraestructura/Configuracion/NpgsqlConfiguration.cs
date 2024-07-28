using Npgsql;
using System.Data.Entity;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	internal class NpgsqlConfiguration : DbConfiguration
	{
		public NpgsqlConfiguration()
		{
			SetProviderServices("Npgsql", NpgsqlServices.Instance);
			SetProviderFactory("Npgsql", NpgsqlFactory.Instance);
			SetDefaultConnectionFactory(new NpgsqlConnectionFactory());
		}
	}
}
