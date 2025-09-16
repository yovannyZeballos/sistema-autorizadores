using Oracle.ManagedDataAccess.Client;

namespace SGP.Api.Services
{
    public class HPCT02Service
    {
        private readonly string _conexionHPCT02;
        public HPCT02Service(IConfiguration configuration)
        {
            _conexionHPCT02 = configuration.GetConnectionString("HPCT02");
        }

        public async Task<string> ObtenerFechaNegocio()
        {
            string fechaNegocio = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                string query = @"SELECT DPROCESO FROM CPRFECCD";

                using (OracleConnection connection = new OracleConnection(_conexionHPCT02))
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
                throw new Exception("Error al obtener fecha negocio HPCT02::" + ex.Message);
            }
        }

        public async Task<string> ObtenerEstadoConexion()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_conexionHPCT02))
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
