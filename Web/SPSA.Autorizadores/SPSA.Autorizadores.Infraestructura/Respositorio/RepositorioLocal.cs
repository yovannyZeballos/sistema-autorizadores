using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System.Data.SqlClient;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioLocal : CadenasConexion, IRepositorioLocal
    {
        private readonly int _commandTimeout;

        public RepositorioLocal()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task<List<Local>> ListarXEmpresa(string ruc)
        {
            var locales = new List<Local>();
            using (var connection = new SqlConnection(CadenaConexionCarteleria))
            {
                var command = new SqlCommand("SP_AUTORIZADOR_LISTAR_LOCALES", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 11,
                    Value = ruc,
                    ParameterName = "@Ruc"
                });

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dr != null && dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        locales.Add(new Local
                        {
                            Codigo = dr["COD_LOCAL"].ToString(),
                            Nombre = dr["NOM_LOCAL"].ToString()
                        });
                    }
                }
                connection.Close();
                connection.Dispose();
                return locales;
            }
        }

        public async Task<Local> ObtenerCarteleria(string codLocal)
        {
            var local = new Local();
            using (var connection = new SqlConnection(CadenaConexionCarteleria))
            {
                var command = new SqlCommand("SP_AUTORIZADOR_OBTENER_LOCAL", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 11,
                    Value = codLocal,
                    ParameterName = "@COD_LOCAL"
                });

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dr != null && dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        local.Codigo = dr["COD_LOCAL"].ToString();
                        local.Nombre = dr["NOM_LOCAL"].ToString();
                        local.TipoSO = dr["TIP_OS"].ToString();
                    }
                }
                connection.Close();
                connection.Dispose();
                return local;
            }
        }

        public async Task<Local> Obtener(int codigo)
        {
            var local = new Local();
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_MANT_LISTA_LOCALES", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add("vCOD_LOCAL", OracleDbType.Int16, codigo, ParameterDirection.Input);
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dr != null && dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        local.Manual = dr["Manual"].ToString();
                        local.Nombre = dr["nLocal"].ToString();
                        local.Codigo = dr["cLocal"].ToString();
                        local.Estado = dr["Estado"].ToString();
                        local.CodigoOfiplan = dr["nLocalOfiplan"].ToString();
                    }
                }
                connection.Close();
                connection.Dispose();
                return local;
            }
        }

        public async Task<DataTable> ListaLocalesAsignar()
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_G4_LISTA_LOCALES", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);


                connection.Close();
                connection.Dispose();

                return datatable;
            }
        }

        public async Task AsignarLocal(string codLocal, string codCadena)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_B_ASIGNAR_LOCAL", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout;

                await command.Connection.OpenAsync();
                command.Parameters.Add("vCOD_CADENA", OracleDbType.Varchar2, codCadena, ParameterDirection.Input);
                command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codLocal, ParameterDirection.Input);
                command.Parameters.Add("nRESP", OracleDbType.Decimal, 1, ParameterDirection.Output);
                command.Parameters.Add("vMENSAJE", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

                await command.ExecuteNonQueryAsync();

                var error = Convert.ToDecimal(command.Parameters["nRESP"].Value.ToString());
                var mensjaeError = command.Parameters["vMENSAJE"].Value.ToString();

                if (error == -1)
                    throw new Exception(mensjaeError);

                connection.Close();
                connection.Dispose();
            }
        }
    }
}
