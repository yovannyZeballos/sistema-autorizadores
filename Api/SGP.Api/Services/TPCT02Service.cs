using Oracle.ManagedDataAccess.Client;

namespace SGP.Api.Services
{
    public class TPCT02Service
    {
        private readonly string _conexionTPCT02;
        public TPCT02Service(IConfiguration configuration)
        {
            _conexionTPCT02 = configuration.GetConnectionString("TPCT02");
        }

        public async Task<string> ObtenerFechaNegocio()
        {
            string fechaNegocio = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                string query = @"SELECT DPROCESO FROM CPRFECCD";

                using (OracleConnection connection = new OracleConnection(_conexionTPCT02))
                {
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        await connection.OpenAsync();

                        var result = await command.ExecuteScalarAsync();
                        if (result != null && DateTime.TryParse(result.ToString(), out DateTime date))
                        {
                            fechaNegocio = date.ToString("yyyy-MM-dd") + " 00:00:00 ";
                        }
                    }
                }
                return fechaNegocio;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener fecha negocio TPCT02::" + ex.Message);
            }
        }

        public async Task<string> ObtenerEstadoConexion()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_conexionTPCT02))
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
