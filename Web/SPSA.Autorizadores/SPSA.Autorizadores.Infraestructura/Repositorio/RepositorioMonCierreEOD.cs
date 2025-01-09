using Npgsql;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioMonCierreEOD : RepositorioGenerico<SGPContexto, MonCierreEOD>, IRepositorioMonCierreEOD
	{
		public RepositorioMonCierreEOD(SGPContexto context) : base(context) { }

		public SGPContexto SGPContext
		{
			get { return _contexto; }
		}

		public async Task<List<Mae_Local>> Listar(string codEmpresa, DateTime fecha, int tipo)
		{
			List<Mae_Local> locales = new List<Mae_Local>();

			using (var connection = new NpgsqlConnection(SGPContext.Database.Connection.ConnectionString))
			{
				await connection.OpenAsync();

				string query = @"
                    SELECT * FROM ""SGP"".""sf_mon_listar_local""(@p_cod_empresa, @p_fec_cierre, @p_tipo)";

				using (var command = new NpgsqlCommand(query, connection))
				{
					command.Parameters.Add(new NpgsqlParameter("@p_cod_empresa", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codEmpresa });
					command.Parameters.Add(new NpgsqlParameter("@p_fec_cierre", NpgsqlTypes.NpgsqlDbType.Date) { Value = fecha });
					command.Parameters.Add(new NpgsqlParameter("@p_tipo", NpgsqlTypes.NpgsqlDbType.Integer) { Value = tipo });

					using (var dr = await command.ExecuteReaderAsync())
					{
						if (dr != null && dr.HasRows)
						{
							while (await dr.ReadAsync())
							{
								locales.Add(new Mae_Local
								{
									CodEmpresa = dr["cod_empresa"].ToString(),
									CodCadena = dr["cod_cadena"].ToString(),
									CodRegion = dr["cod_region"].ToString(),
									CodZona = dr["cod_zona"].ToString(),
									CodLocal = dr["cod_local"].ToString(),
									Ip = dr["ip"].ToString()
								});
							}
						}
						return locales;
					}
				}
			}
		}
	}
}