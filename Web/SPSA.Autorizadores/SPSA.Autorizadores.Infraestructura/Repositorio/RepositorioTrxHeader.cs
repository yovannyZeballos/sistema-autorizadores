using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioTrxHeader : CadenasConexion, IRepositorioTrxHeader
	{
		private readonly int _commandTimeout;

		public RepositorioTrxHeader()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}

		public async Task<(int cantidadTransacciones, decimal montoFinal)> ObtenerCantidadTransacciones(int local, string fecha)
		{
			using (var connection = new SqlConnection(CadenaConexionBCT))
			{
				var command = new SqlCommand(
					"select count(1) from trxheader where budate = @fecha and branchid = @local and idtransactiontype in (2,10,56); " +
					"select sum(T.amount - T.changeAmount + T.amttnfee) as montofinal from trxheader H inner join trxtender T ON H.trxid = T.trxid where H.budate = @fecha and H.branchid = @local and H.idtransactiontype in (10,56);",
					connection)
				{
					CommandType = CommandType.Text,
					CommandTimeout = _commandTimeout
				};

				command.Parameters.AddWithValue("@fecha", fecha);
				command.Parameters.AddWithValue("@local", local);

				await connection.OpenAsync();

				using (var reader = await command.ExecuteReaderAsync())
				{
					int cantidadTransacciones = 0;
					decimal montoFinal = 0;

					// Leer el primer resultado
					if (await reader.ReadAsync())
					{
						cantidadTransacciones = reader.GetInt32(0);
					}

					// Moverse al siguiente conjunto de resultados
					if (await reader.NextResultAsync() && await reader.ReadAsync())
					{
						montoFinal = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
					}

					return (cantidadTransacciones, montoFinal);
				}
			}
		}
	}
}
