using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System.Collections.Generic;
using System.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioEmpresa : CadenasConexion, IRepositorioEmpresa
    {
        private readonly int _commandTimeout;

        public RepositorioEmpresa()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }


        public async Task<List<Empresa>> Listar()
        {
            var empresas = new List<Empresa>();
            using (SqlConnection connection = new SqlConnection(CadenaConexionCarteleria))
            {
                var command = new SqlCommand("SP_AUTORIZADOR_LISTAR_EMPRESAS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dr != null && dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        empresas.Add(new Empresa
                        {
                            Descripcion = dr["RAZON_SOCIAL"].ToString().Trim(),
                            Ruc = dr["Ruc"].ToString().Trim()
                        });
                    }
                }
                connection.Close();
                connection.Dispose();
                return empresas;
            }
        }

        public async Task<List<Empresa>> ListarOfiplan()
        {
            var empresas = new List<Empresa>();
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_LISTA_EMPRESAS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dr != null && dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        empresas.Add(new Empresa
                        {
                            Ruc = dr["CODIGO"].ToString(),
                            Descripcion = dr["NOMBRE"].ToString(),
                        });
                    }
                }

                connection.Close();
                connection.Dispose();

                return empresas;
            }
        }

        public async Task<List<Empresa>> ListarMonitor()
        {
            var empresas = new List<Empresa>();
            using (SqlConnection connection = new SqlConnection(CadenaConexionCarteleria))
            {
                var command = new SqlCommand("SP_MONI_LST_EMP", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dr != null && dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        empresas.Add(new Empresa
                        {
                            Descripcion = dr["RAZON_SOCIAL"].ToString().Trim(),
                            Ruc = dr["Ruc"].ToString().Trim(),
                            Codigo = dr["COD_EMPRESA"].ToString().Trim()
                        });
                    }
                }
                connection.Close();
                connection.Dispose();
                return empresas;
            }
        }
    }
}
