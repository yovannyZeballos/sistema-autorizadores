using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace SGP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelfCheckoutController : ControllerBase
    {
        private readonly string _conexionSPT03;
        private readonly string _conexionSGP;

        public SelfCheckoutController(IConfiguration configuration)
        {
            _conexionSPT03 = configuration.GetConnectionString("SPT03");
            _conexionSGP = configuration.GetConnectionString("SGP");
        }

        [HttpGet("exportar-pos-trx/{fecha}")]
        public IActionResult ExportarRegistros(string fecha)
        {
            if (!DateTime.TryParse(fecha, out DateTime fechaData))
            {
                fechaData = DateTime.Now.Date;
            }

            try
            {
                using (NpgsqlConnection postgresConnection = new NpgsqlConnection(_conexionSGP))
                {
                    postgresConnection.Open();
                    EliminarRegistrosPorFecha(postgresConnection, fechaData);

                    using (OracleConnection oracleConnection = new OracleConnection(_conexionSPT03))
                    {
                        oracleConnection.Open();
                        string oracleQuery = $@"SELECT l.loc_descripcion, t.hed_fcontable, t.hed_pos, COUNT(*) 
                                                FROM EXCT2SP.ctx_header_trx t, irs_locales l, EXCT2SP.TMP_CAJAS_SELFCHECKOUT x
                                                WHERE hed_pais = 0 AND hed_origentrx = 1
                                                    AND hed_fcontable >= TO_DATE('{fechaData:yyyy-MM-dd}', 'yyyy-MM-dd')
                                                    AND hed_fcontable < TO_DATE('{fechaData:yyyy-MM-dd}', 'yyyy-MM-dd') + INTERVAL '1' DAY
                                                    AND t.hed_tipotrx = 'PVT' AND t.hed_anulado = 'N'
                                                    AND t.hed_local = x.local AND t.hed_pos = x.caja
                                                    AND t.hed_local = l.loc_numero
                                                GROUP BY l.loc_descripcion, t.hed_fcontable, t.hed_pos";

                        using (OracleCommand command = new OracleCommand(oracleQuery, oracleConnection))
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string locDesc = reader.GetString(0);
                                DateTime fContable = reader.GetDateTime(1);
                                int hedPos = reader.GetInt32(2);
                                int totTrx = reader.GetInt32(3);

                                string insertQuery = @"INSERT INTO ""SGP"".""RPTE_SELFCHECKOUT""
                                                        (""LOC_DESCRIPCION"", ""HED_FCONTABLE"", ""HED_POS"", ""TOT_TRX"")
                                                        VALUES(@LOC_DESCRIPCION, @HED_FCONTABLE, @HED_POS, @TOT_TRX);";

                                InsertarData(postgresConnection, insertQuery, locDesc, fContable, hedPos, totTrx);
                            }
                        }
                    }
                }

                return Ok("Exportación completada correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private void EliminarRegistrosPorFecha(NpgsqlConnection connection, DateTime fechaProcesar)
        {
            string deleteQuery = @"DELETE FROM ""SGP"".""RPTE_SELFCHECKOUT"" 
                                    WHERE ""HED_FCONTABLE"" = @FechaProcesar;";

            using (NpgsqlCommand deleteCommand = new NpgsqlCommand(deleteQuery, connection))
            {
                deleteCommand.Parameters.AddWithValue("@FechaProcesar", fechaProcesar);
                deleteCommand.ExecuteNonQuery();
                Console.WriteLine($"Se eliminaron los registros de {fechaProcesar:yyyy-MM-dd}");
            }
        }

        private void InsertarData(NpgsqlConnection connection, string query, string locDesc, DateTime fContable, int hedPos, int totTrx)
        {
            using (NpgsqlCommand insertCommand = new NpgsqlCommand(query, connection))
            {
                insertCommand.Parameters.AddWithValue("@LOC_DESCRIPCION", locDesc);
                insertCommand.Parameters.AddWithValue("@HED_FCONTABLE", fContable);
                insertCommand.Parameters.AddWithValue("@HED_POS", hedPos);
                insertCommand.Parameters.AddWithValue("@TOT_TRX", totTrx);
                insertCommand.ExecuteNonQuery();
                Console.WriteLine($"{locDesc} | {fContable:yyyy-MM-dd} | {hedPos} | {totTrx}");
            }
        }
    }
}
