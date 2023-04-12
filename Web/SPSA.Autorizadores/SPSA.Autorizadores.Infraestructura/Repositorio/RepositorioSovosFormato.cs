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
    public class RepositorioSovosFormato : CadenasConexion, IRepositorioSovosFormato
    {
        private readonly int _commandTimeout;

        public RepositorioSovosFormato()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task<List<SovosFormato>> LocListar(string codEmpresa)
        {
            var formatos = new List<SovosFormato>();
            using (SqlConnection connection = new SqlConnection(CadenaConexionCarteleria))
            {
                var command = new SqlCommand("SP_LOC_LIST_FORMATO", connection)
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

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (dr != null && dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        formatos.Add(new SovosFormato
                        {
                            CodFormato = dr["COD_FORMATO"].ToString().Trim(),
                            Nombre = dr["NOM_FORMATO"].ToString().Trim()
                        });
                    }
                }
                connection.Close();
                connection.Dispose();
                return formatos;
            }
        }
    }
}
