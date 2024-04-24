using Npgsql;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioDataTable : CadenasConexion, IRepositorioDataTable
    {
        private readonly int _commandTimeout;
        string connectionString = ConfigurationManager.ConnectionStrings["SGP"].ConnectionString;

        public RepositorioDataTable()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task<DataTable> ListarUsuarios()
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string sql = @"SELECT * FROM ""SGP"".sf_listar_seg_usuario();";

            var dataTable = new DataTable();

            using (var cmd = new NpgsqlCommand(sql, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }

            connection.Close();

            return dataTable;
        }
    }
}
