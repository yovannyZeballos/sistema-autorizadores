﻿using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Utiles
{
    public class DBHelper
    {
        public string CadenaConexion { get; set; }
        private readonly int _commandTimeout;

        public DBHelper()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }


        public SqlParameter MakeParam(string paramName, object value, SqlDbType dbType, ParameterDirection direction, int size = 0)
        {
            if (size > 0)
                return new SqlParameter
                {
                    SqlDbType = dbType,
                    Direction = direction,
                    Size = size,
                    Value = value,
                    ParameterName = paramName
                };
            else
                return new SqlParameter
                {
                    SqlDbType = dbType,
                    Direction = direction,
                    Value = value,
                    ParameterName = paramName
                };
        }

        public async Task ExecuteNonQuery(string query, SqlParameter[] dbParams = null)
        {
            using (var connection = new SqlConnection(CadenaConexion))
            {
                var command = new SqlCommand(query, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                if (dbParams != null)
                    command.Parameters.AddRange(dbParams);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Dictionary<string, object>> ExecuteNonQueryWithOutput(string query, SqlParameter[] dbParams)
        {
            var outputs = new Dictionary<string, object>();
            using (var connection = new SqlConnection(CadenaConexion))
            {
                var command = new SqlCommand(query, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.AddRange(dbParams);

                await command.ExecuteNonQueryAsync();

                foreach (var param in dbParams.Where(x => x.Direction == ParameterDirection.Output))
                {
                    outputs.Add(param.ParameterName, command.Parameters[param.ParameterName].Value);
                }

                return outputs;
            }
        }

        public async Task<SqlDataReader> ExecuteReader(string query, SqlParameter[] dbParams = null)
        {
            var connection = new SqlConnection(CadenaConexion);

            using (var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = _commandTimeout
            })
            {
                await command.Connection.OpenAsync();

                if (dbParams != null)
                    command.Parameters.AddRange(dbParams);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                return dr;
            }
        }

    }
}