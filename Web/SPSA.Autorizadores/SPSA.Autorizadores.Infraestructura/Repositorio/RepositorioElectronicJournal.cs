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
				var command = new NpgsqlCommand("select trx_data from mtxadmin.electronicjournal where date = @fecha and  action_code in (0,4) and action_sub_code in (0,0)", connection)
				{
					CommandType = CommandType.Text,
					CommandTimeout = _commandTimeout
				};

				command.Parameters.AddWithValue("fecha", fecha);

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
