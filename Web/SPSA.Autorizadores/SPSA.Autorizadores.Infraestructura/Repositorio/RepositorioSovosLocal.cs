using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioSovosLocal : CadenasConexion, IRepositorioSovosLocal
    {
        private readonly int _commandTimeout;

        public RepositorioSovosLocal()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task<List<SovosLocal>> Listar(string codEmpresa, DateTime fecha)
        {
            using (var connection = new SqlConnection(CadenaConexionCarteleria))
            {
                var locales = new List<SovosLocal>();

                var command = new SqlCommand("SP_MONI_LISTAR_SOVOS_LOCAL", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 10,
                    Value = codEmpresa,
                    ParameterName = "@COD_EMPRESA"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = fecha,
                    ParameterName = "@FEC_CIERRE"
                });

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dr != null && dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        locales.Add(new SovosLocal
                        {
                            CodLocal = dr["COD_LOCAL"].ToString(),
                            Ip = dr["IP_ADDRES"].ToString(),
                            CodFormato = dr["COD_FORMATO"].ToString()
                        });
                    }
                }
                connection.Close();
                connection.Dispose();
                return locales;
            }
        }
    }
}
