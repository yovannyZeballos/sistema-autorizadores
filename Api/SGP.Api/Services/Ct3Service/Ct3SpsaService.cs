using Npgsql;

namespace SGP.Api.Services.Ct3Service
{
    public class Ct3SpsaService
    {
        private readonly string _conexionCt3;
        public Ct3SpsaService(IConfiguration configuration)
        {
            _conexionCt3 = configuration.GetConnectionString("CT3_SP") ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'CT3_SP' not found.");
        }


        public async Task<string> ObtenerFechaNegocio()
        {
            string fechaHoraNegocio = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                string query = @"SELECT dproceso FROM exct.cprfeccd LIMIT 1";

                await using (var connection = new NpgsqlConnection(_conexionCt3))
                await using (var command = new NpgsqlCommand(query, connection))
                {
                    await connection.OpenAsync();

                    var result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        DateTime fecha = Convert.ToDateTime(result);
                        fechaHoraNegocio = fecha.ToString("yyyy-MM-dd");
                    }
                }

                return fechaHoraNegocio;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener fecha negocio (Ct3 Spsa):: " + ex.Message);
            }
        }

        public async Task<string> ObtenerEstadoConexion()
        {
            try
            {
                await using (var connection = new NpgsqlConnection(_conexionCt3))
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
