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

		public async Task<(int cantidadTransacciones, decimal montoFinal)> ListarTransacciones(string cdenaConexion, string fecha)
		{
			var query = $@"
						WITH transaction_data AS (
							SELECT REPLACE(trx_data,'S/.','') trx_data
							FROM  mtxadmin.electronicjournal WHERE date = '{fecha}' 
						AND action_code in (0,4) AND action_sub_code IN (0) AND trx_data 
						NOT LIKE '%CANCELADO%' AND trx_data NOT LIKE '%SUSPENDER%'  
						 AND trx_data NOT LIKE '%ARQUEO%' AND trx_data NOT LIKE '%ARTICULOS       0%'
						),
						parsed_data AS (
							SELECT
								trx_data,
								COALESCE((SELECT (regexp_matches(trx_data, 'T O T A L\s+(-?[\d\.]+)', 'g'))[1]::numeric), 0) AS total,
								COALESCE((SELECT (regexp_matches(trx_data, 'PERU CHAMPS\s+(-?[\d\.]+)', 'g'))[1]::numeric), 0) AS peru_champs,
								COALESCE((SELECT (regexp_matches(trx_data, '\bTELETON\b\s+(-?[\d\.]+)', 'g'))[1]::numeric), 0) AS teleton,
								COALESCE((SELECT (regexp_matches(trx_data, 'TELETON PERU\s+(-?[\d\.]+)', 'g'))[1]::numeric), 0) AS teleton_peru,
								COALESCE((SELECT (regexp_matches(trx_data, 'APOYO A LA TELETON\s+(-?[\d\.]+)', 'g'))[1]::numeric), 0) AS teleton_apoyo,
								COALESCE((SELECT (regexp_matches(trx_data, 'APOYA A LA TELETON\s+IMPORTE:\s+(\d+\.\d+)', 'g'))[1]::numeric), 0) AS teleton_apoyo_importe,
								COALESCE((SELECT (regexp_matches(trx_data, 'EXC.REDONDEO\s+(-?[\d\.]+)', 'g'))[1]::numeric), 0) AS exc_redondeo
							FROM transaction_data
						),
						final_amount AS (
							SELECT
								trx_data,
								CASE
									WHEN total >= 0 THEN total + peru_champs + teleton + teleton_peru + teleton_apoyo + teleton_apoyo_importe - exc_redondeo
										ELSE 0
								END AS final_amount
							FROM parsed_data
						)
						SELECT sum(final_amount) MontoFinal
						FROM final_amount;

						SELECT COUNT(1) Cantidad FROM mtxadmin.electronicjournal WHERE date = '{fecha}' AND action_code in (0,4) 
						AND action_sub_code IN (0) AND trx_data NOT LIKE '%CANCELADO%' AND trx_data NOT LIKE '%SUSPENDER%' 
						AND trx_data NOT LIKE '%ARTICULOS       0%'
			";

			var transacciones = new List<ElectronicJournal>();

			using (NpgsqlConnection connection = new NpgsqlConnection(cdenaConexion))
			{
				var command = new NpgsqlCommand(query, connection)
				{
					CommandType = CommandType.Text,
					CommandTimeout = _commandTimeout
				};


				await command.Connection.OpenAsync();
				using (var reader = await command.ExecuteReaderAsync())
				{
					int cantidadTransacciones = 0;
					decimal montoFinal = 0;

					// Leer el primer resultado
					if (await reader.ReadAsync())
					{
						montoFinal = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
					}

					// Moverse al siguiente conjunto de resultados
					if (await reader.NextResultAsync() && await reader.ReadAsync())
					{
						cantidadTransacciones = reader.GetInt32(0);
					}

					return (cantidadTransacciones, montoFinal);
				}

			}
		}
	}
}
