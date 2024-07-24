using Npgsql;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioTmpMonCierreLocal : RepositorioGenerico<SGPContexto, TmpMonCierreLocal>, IRepositorioTmpMonCierreLocal
	{
		public RepositorioTmpMonCierreLocal(SGPContexto context) : base(context) { }

		public SGPContexto BCTContexto
		{
			get { return _contexto; }
		}

		public async Task BulkInsert(IEnumerable<TmpMonCierreLocal> entities)
		{
			var conn = (NpgsqlConnection)_contexto.Database.Connection;

			if(conn.State == System.Data.ConnectionState.Closed)
				await conn.OpenAsync();

			using (var writer = conn.BeginTextImport("COPY \"SGP\".\"TMP_MON_CIERRE_LOCAL\" (\"COD_LOCAL_ALTERNO\", \"FEC_CONTABLE\", \"FEC_CIERRE\", \"TIP_ESTADO\", \"FEC_CARGA\", \"HOR_CARGA\") FROM STDIN (FORMAT CSV)"))
			{
				foreach (var entity in entities)
				{
					writer.WriteLine($"{entity.CodLocalAlterno},{entity.FechaContable?.ToString("yyyy-MM-dd")},{entity.FechaCierre?.ToString("yyyy-MM-dd")},{entity.TipEstado},{entity.FechaCarga:yyyy-MM-dd},{entity.HoraCarga}");
				}

			}

			await conn.CloseAsync();
		}

		public async Task Truncate()
		{
			await _contexto.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE \"SGP\".\"TMP_MON_CIERRE_LOCAL\" RESTART IDENTITY CASCADE");
		}
	}
}
