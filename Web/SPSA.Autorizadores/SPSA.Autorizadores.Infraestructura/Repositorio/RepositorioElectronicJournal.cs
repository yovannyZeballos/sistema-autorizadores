using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Configuration;
using System;
using System.Threading.Tasks;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Npgsql;
using System.Windows.Input;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioElectronicJournal : IRepositorioElectronicJournal
	{
		private readonly int _commandTimeout;

		public RepositorioElectronicJournal()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}

		public async Task<List<ElectronicJournal>> ListarTransacciones(string cdenaConexion, string fecha)
		{
			
			var transacciones = new List<ElectronicJournal>();

			using (NpgsqlConnection connection = new NpgsqlConnection(cdenaConexion))
			{
				var command = new NpgsqlCommand($"SELECT trx_data FROM mtxadmin.electronicjournal WHERE date = '{fecha}' AND action_code in (0,4) AND action_sub_code IN (0) AND trx_data NOT LIKE '%CANCELADO%' AND trx_data NOT LIKE '%SUSPENDER%' AND trx_data NOT LIKE '%ARTICULOS       0%'", connection)
				{
					CommandType = CommandType.Text,
					CommandTimeout = _commandTimeout
				};


				await command.Connection.OpenAsync();

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

				if (dr != null && dr.HasRows)
				{
					while (await dr.ReadAsync())
					{
						transacciones.Add(new ElectronicJournal
						{
							TrxData = dr["trx_data"].ToString().Trim(),
						});
					}
				}
				connection.Close();
				connection.Dispose();
				return transacciones;
			}
		}
	}
}
