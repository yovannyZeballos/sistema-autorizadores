using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using SGP.Api.Models;

namespace SGP.Api.Services
{
    public class SPT03Service
    {
        private readonly string _conexionSPT03;
        public SPT03Service(IConfiguration configuration)
        {
            _conexionSPT03 = configuration.GetConnectionString("SPT03");
        }

        public List<IRS_LOCALES> ObtenerLocalesCT2()
        {
            List<IRS_LOCALES> listaLocales = new List<IRS_LOCALES>();

            string query = @"SELECT NVL(LOC_NUMERO, '0') AS LOC_NUMERO,
                               NVL(LOC_DESCRIPCION, '') AS LOC_DESCRIPCION,
                               NVL(REG_NUMERO, '0') AS REG_NUMERO,
                               NVL(CAD_NUMERO, '0') AS CAD_NUMERO,
                               NVL(LOC_CENTROCOSTO, '') AS LOC_CENTROCOSTO,
                               NVL(LOC_SOCIEDAD, '') AS LOC_SOCIEDAD,
                               NVL(LOC_NUMEROSAP, '') AS LOC_NUMEROSAP,
                               NVL(LOC_ACTIVO, '') AS LOC_ACTIVO,
                               NVL(LOC_NUMEROPMM, '') AS LOC_NUMEROPMM,
                               NVL(LOC_TIPO, '') AS LOC_TIPO,
                               NVL(LOC_FINICIO, '') AS LOC_FINICIO,
                               NVL(LOC_CODSUNAT, '') AS LOC_CODSUNAT
                        FROM ECT2SP.IRS_LOCALES
                        WHERE LOC_NUMERO BETWEEN '1' AND '4999'
                        ORDER BY LOC_NUMERO ASC";

            using (OracleConnection connection = new OracleConnection(_conexionSPT03))
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        OracleDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            IRS_LOCALES local = new IRS_LOCALES
                            {
                                LOC_NUMERO = reader["LOC_NUMERO"].ToString(),
                                LOC_DESCRIPCION = reader["LOC_DESCRIPCION"].ToString(),
                                REG_NUMERO = reader["REG_NUMERO"].ToString(),
                                CAD_NUMERO = Convert.ToInt32(reader["CAD_NUMERO"]),
                                LOC_CENTROCOSTO = reader["LOC_CENTROCOSTO"].ToString(),
                                LOC_SOCIEDAD = reader["LOC_SOCIEDAD"].ToString(),
                                LOC_NUMEROSAP = reader["LOC_NUMEROSAP"].ToString(),
                                LOC_ACTIVO = reader["LOC_ACTIVO"].ToString(),
                                LOC_NUMEROPMM = reader["LOC_NUMEROPMM"].ToString(),
                                LOC_TIPO = reader["LOC_TIPO"].ToString(),
                                LOC_CODSUNAT = reader["LOC_CODSUNAT"].ToString(),
                                LOC_FINICIO = reader.IsDBNull(reader.GetOrdinal("LOC_FINICIO")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LOC_FINICIO")),
                            };

                           

                            listaLocales.Add(local);
                        }

                        reader.Close();

                        return listaLocales;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error al obtener lista locales CT2::" + ex.Message);
                    }
                }
            }
        }

        public async Task<string> ObtenerFechaNegocio()
        {
            string fechaHoraNegocio = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string query = @"SELECT a.DPROCESO, b.hora_carga
                         FROM CPRFECCD a, ECTSP.HORA_CARGA b
                         WHERE a.DPROCESO = b.fecha_carga";

                using (OracleConnection connection = new OracleConnection(_conexionSPT03))
                {
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        await connection.OpenAsync();

                        using (OracleDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                DateTime fecha = reader.GetDateTime(0);
                                string hora = reader.GetString(1);

                                if (TimeSpan.TryParse(hora, out TimeSpan horaTimeSpan))
                                {
                                    DateTime fechaYHora = fecha.Date + horaTimeSpan;
                                    fechaHoraNegocio = fechaYHora.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                else
                                {
                                    fechaHoraNegocio = fecha.ToString("yyyy-MM-dd") + " " + hora;
                                }
                            }
                        }
                    }
                }
                return fechaHoraNegocio;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener fecha negocio SPT03::" + ex.Message);
            }
        }

        public async Task<string> ObtenerEstadoConexion()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_conexionSPT03))
                {
                    await connection.OpenAsync();
                    return "SI";
                }
            }
            catch
            {
                return "NO";
            }
        }
    }
}
