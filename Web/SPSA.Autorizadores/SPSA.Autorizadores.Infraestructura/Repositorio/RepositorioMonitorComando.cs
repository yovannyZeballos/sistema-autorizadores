using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioMonitorComando : CadenasConexion, IRepositorioMonitorComando
	{

		private readonly int _commandTimeout;

		public RepositorioMonitorComando()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}

		public async Task<List<MonitorComando>> ListarPorTipo(int tipo)
		{
			var comandos = new List<MonitorComando>();
			using (var connection = new SqlConnection(CadenaConexionCarteleria))
			{
				var command = new SqlCommand("SP_MONI_LISTAR_COMANDOS", connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = _commandTimeout
				};

				await command.Connection.OpenAsync();
				command.Parameters.Add(new SqlParameter
				{
					SqlDbType = SqlDbType.Int,
					Direction = ParameterDirection.Input,
					Value = tipo,
					ParameterName = "@Tipo"
				});

				var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

				if (dr != null && dr.HasRows)
				{
					while (await dr.ReadAsync())
					{
						comandos.Add(new MonitorComando
						{
							Codigo = dr["CODIGO"].ToString(),
							Comando = dr["COMANDO"].ToString(),
							Id = Convert.ToInt32(dr["ID"]),
							Tipo = Convert.ToInt32(dr["TIPO"]),
							Descripcion = dr["DESCRIPCION"].ToString()
						});
					}
				}
				connection.Close();
				connection.Dispose();
				return comandos;
			}
		}
	}
}
