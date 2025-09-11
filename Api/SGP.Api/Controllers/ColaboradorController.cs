using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SGP.Api.Models;

namespace SGP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColaboradorController : ControllerBase
    {
        private readonly string _conexionSPT03;
        private readonly string _conexionSGP;

        public ColaboradorController(IConfiguration configuration)
        {
            _conexionSPT03 = configuration.GetConnectionString("SPT03");
            _conexionSGP = configuration.GetConnectionString("SGP");
        }

        [HttpGet("actualizar-colaboradores-sgp")]
        public IActionResult ActualizarColaboradores()
        {
            try
            {
                using (NpgsqlConnection postgresConnection = new NpgsqlConnection(_conexionSGP))
                {
                    postgresConnection.Open();

                    TruncateTabla(postgresConnection);

                    using (OracleConnection oracleConnection = new OracleConnection(_conexionSPT03))
                    {
                        oracleConnection.Open();
                        string oracleQuery = "SELECT * FROM EAUTORIZADOR.INT_DATOS_COLABORADOR";
                        //string oracleQuery = "SELECT * FROM EAUTORIZADOR.INT_DATOS_COLABORADOR WHERE ROWNUM <= 10";

                        using (OracleCommand command = new OracleCommand(oracleQuery, oracleConnection))
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                DatosColaborador datosColaborador = new()
                                {
                                    co_empr = reader["CO_EMPR"].ToString(),
                                    de_nomb = reader["DE_NOMB"].ToString(),
                                    codigo_ofisis = reader["CODIGO_OFISIS"].ToString(),
                                    no_apel_pate = reader["NO_APEL_PATE"].ToString(),
                                    no_apel_mate = reader["NO_APEL_MATE"].ToString(),
                                    no_trab = reader["NO_TRAB"].ToString(),
                                    ti_docu_iden = reader["TI_DOCU_IDEN"].ToString(),
                                    nu_docu_iden = reader["NU_DOCU_IDEN"].ToString(),
                                    fe_ingr_empr = reader.IsDBNull(reader.GetOrdinal("FE_INGR_EMPR")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FE_INGR_EMPR")),
                                    fe_cese_trab = reader.IsDBNull(reader.GetOrdinal("FE_CESE_TRAB")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FE_CESE_TRAB")),
                                    co_plan = reader["CO_PLAN"].ToString(),
                                    de_plan = reader["DE_PLAN"].ToString(),
                                    ti_situ = reader["TI_SITU"].ToString(),
                                    co_pues_trab = reader["CO_PUES_TRAB"].ToString(),
                                    de_pues_trab = reader["DE_PUES_TRAB"].ToString(),
                                    co_sede = reader["CO_SEDE"].ToString(),
                                    de_sede = reader["DE_SEDE"].ToString(),
                                    co_depa = reader["CO_DEPA"].ToString(),
                                    de_depa = reader["DE_DEPA"].ToString(),
                                    co_area = reader["CO_AREA"].ToString(),
                                    de_area = reader["DE_AREA"].ToString(),
                                    co_secc = reader["CO_SECC"].ToString(),
                                    de_secc = reader["DE_SECC"].ToString(),
                                    co_moti_sepa = reader["CO_MOTI_SEPA"].ToString()
                                };

                                string insertQuery = @"
                                                INSERT INTO ""SGP"".""INT_COLABORADOR"" (
                                                    ""CO_EMPR"",
                                                    ""DE_NOMB"",
                                                    ""CODIGO_OFISIS"",
                                                    ""NO_APEL_PATE"",
                                                    ""NO_APEL_MATE"",
                                                    ""NO_TRAB"",
                                                    ""TI_DOCU_IDEN"",
                                                    ""NU_DOCU_IDEN"",
                                                    ""FE_INGR_EMPR"",
                                                    ""FE_CESE_TRAB"",
                                                    ""CO_PLAN"",
                                                    ""DE_PLAN"",
                                                    ""TI_SITU"",
                                                    ""CO_PUES_TRAB"",
                                                    ""DE_PUES_TRAB"",
                                                    ""CO_SEDE"",
                                                    ""DE_SEDE"",
                                                    ""CO_DEPA"",
                                                    ""DE_DEPA"",
                                                    ""CO_AREA"",
                                                    ""DE_AREA"",
                                                    ""CO_SECC"",
                                                    ""DE_SECC"",
                                                    ""CO_MOTI_SEPA""
                                                ) VALUES (
                                                    :CO_EMPR,
                                                    :DE_NOMB,
                                                    :CODIGO_OFISIS,
                                                    :NO_APEL_PATE,
                                                    :NO_APEL_MATE,
                                                    :NO_TRAB,
                                                    :TI_DOCU_IDEN,
                                                    :NU_DOCU_IDEN,
                                                    :FE_INGR_EMPR,
                                                    :FE_CESE_TRAB,
                                                    :CO_PLAN,
                                                    :DE_PLAN,
                                                    :TI_SITU,
                                                    :CO_PUES_TRAB,
                                                    :DE_PUES_TRAB,
                                                    :CO_SEDE,
                                                    :DE_SEDE,
                                                    :CO_DEPA,
                                                    :DE_DEPA,
                                                    :CO_AREA,
                                                    :DE_AREA,
                                                    :CO_SECC,
                                                    :DE_SECC,
                                                    :CO_MOTI_SEPA
                                                )";

                                //const string insertQuery = @"
                                //INSERT INTO ""SGP"".int_datos_colaborador (
                                //    co_empr, de_nomb, codigo_ofisis, no_apel_pate, no_apel_mate, no_trab,
                                //    ti_docu_iden, nu_docu_iden, fe_ingr_empr, fe_cese_trab,
                                //    co_plan, de_plan, ti_situ, co_pues_trab, de_pues_trab,
                                //    co_sede, de_sede, co_depa, de_depa, co_area, de_area,
                                //    co_secc, de_secc, co_moti_sepa
                                //) VALUES (
                                //    :co_empr, :de_nomb, :codigo_ofisis, :no_apel_pate, :no_apel_mate, :no_trab,
                                //    :ti_docu_iden, :nu_docu_iden, :fe_ingr_empr, :fe_cese_trab,
                                //    :co_plan, :de_plan, :ti_situ, :co_pues_trab, :de_pues_trab,
                                //    :co_sede, :de_sede, :co_depa, :de_depa, :co_area, :de_area,
                                //    :co_secc, :de_secc, :co_moti_sepa
                                //);";


                                InsertarData(postgresConnection, insertQuery, datosColaborador);
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

        //private static void TruncateTabla(NpgsqlConnection connection)
        //{
        //    const string deleteQuery = @"TRUNCATE TABLE ""SGP"".int_datos_colaborador CONTINUE IDENTITY RESTRICT;";
        //    using var deleteCommand = new NpgsqlCommand(deleteQuery, connection);
        //    deleteCommand.ExecuteNonQuery();
        //}

        //private static void InsertarData(NpgsqlConnection connection, string query, DatosColaborador datos)
        //{
        //    using var insertCommand = new NpgsqlCommand(query, connection);

        //    insertCommand.Parameters.AddWithValue(":co_empr", (object?)datos.co_empr ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":de_nomb", (object?)datos.de_nomb ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":codigo_ofisis", (object?)datos.codigo_ofisis ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":no_apel_pate", (object?)datos.no_apel_pate ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":no_apel_mate", (object?)datos.no_apel_mate ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":no_trab", (object?)datos.no_trab ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":ti_docu_iden", (object?)datos.ti_docu_iden ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":nu_docu_iden", (object?)datos.nu_docu_iden ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":fe_ingr_empr", (object?)datos.fe_ingr_empr ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":fe_cese_trab", (object?)datos.fe_cese_trab ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":co_plan", (object?)datos.co_plan ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":de_plan", (object?)datos.de_plan ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":ti_situ", (object?)datos.ti_situ ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":co_pues_trab", (object?)datos.co_pues_trab ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":de_pues_trab", (object?)datos.de_pues_trab ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":co_sede", (object?)datos.co_sede ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":de_sede", (object?)datos.de_sede ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":co_depa", (object?)datos.co_depa ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":de_depa", (object?)datos.de_depa ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":co_area", (object?)datos.co_area ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":de_area", (object?)datos.de_area ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":co_secc", (object?)datos.co_secc ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":de_secc", (object?)datos.de_secc ?? DBNull.Value);
        //    insertCommand.Parameters.AddWithValue(":co_moti_sepa", (object?)datos.co_moti_sepa ?? DBNull.Value);

        //    insertCommand.ExecuteNonQuery();

        //    Console.WriteLine($"{datos.codigo_ofisis} | {datos.no_trab} | {datos.no_apel_pate} | {datos.no_apel_mate}");
        //}

        private static void TruncateTabla(NpgsqlConnection connection)
        {
            string deleteQuery = @"TRUNCATE TABLE ""SGP"".""INT_COLABORADOR"" CONTINUE IDENTITY RESTRICT;";

            using (NpgsqlCommand deleteCommand = new NpgsqlCommand(deleteQuery, connection))
            {
                deleteCommand.ExecuteNonQuery();
            }
        }

        private static void InsertarData(NpgsqlConnection connection, string query, DatosColaborador datos)
        {
            using (NpgsqlCommand insertCommand = new NpgsqlCommand(query, connection))
            {

                insertCommand.Parameters.AddWithValue(":CO_EMPR", datos.co_empr);
                insertCommand.Parameters.AddWithValue(":DE_NOMB", datos.de_nomb);
                insertCommand.Parameters.AddWithValue(":CODIGO_OFISIS", datos.codigo_ofisis);
                insertCommand.Parameters.AddWithValue(":NO_APEL_PATE", datos.no_apel_pate);
                insertCommand.Parameters.AddWithValue(":NO_APEL_MATE", datos.no_apel_mate);
                insertCommand.Parameters.AddWithValue(":NO_TRAB", datos.no_trab);
                insertCommand.Parameters.AddWithValue(":TI_DOCU_IDEN", datos.ti_docu_iden);
                insertCommand.Parameters.AddWithValue(":NU_DOCU_IDEN", datos.nu_docu_iden);
                insertCommand.Parameters.AddWithValue(":FE_INGR_EMPR", (object)datos.fe_ingr_empr ?? DBNull.Value);
                insertCommand.Parameters.AddWithValue(":FE_CESE_TRAB", (object)datos.fe_cese_trab ?? DBNull.Value);
                insertCommand.Parameters.AddWithValue(":CO_PLAN", datos.co_plan);
                insertCommand.Parameters.AddWithValue(":DE_PLAN", datos.de_plan);
                insertCommand.Parameters.AddWithValue(":TI_SITU", datos.ti_situ);
                insertCommand.Parameters.AddWithValue(":CO_PUES_TRAB", datos.co_pues_trab);
                insertCommand.Parameters.AddWithValue(":DE_PUES_TRAB", datos.de_pues_trab);
                insertCommand.Parameters.AddWithValue(":CO_SEDE", datos.co_sede);
                insertCommand.Parameters.AddWithValue(":DE_SEDE", datos.de_sede);
                insertCommand.Parameters.AddWithValue(":CO_DEPA", datos.co_depa);
                insertCommand.Parameters.AddWithValue(":DE_DEPA", datos.de_depa);
                insertCommand.Parameters.AddWithValue(":CO_AREA", datos.co_area);
                insertCommand.Parameters.AddWithValue(":DE_AREA", datos.de_area);
                insertCommand.Parameters.AddWithValue(":CO_SECC", datos.co_secc);
                insertCommand.Parameters.AddWithValue(":DE_SECC", datos.de_secc);
                insertCommand.Parameters.AddWithValue(":CO_MOTI_SEPA", datos.co_moti_sepa);
                insertCommand.ExecuteNonQuery();
                Console.WriteLine($"{datos.codigo_ofisis} | {datos.no_trab} | {datos.no_apel_pate} | {datos.no_apel_mate}");
            }
        }

    }
}
