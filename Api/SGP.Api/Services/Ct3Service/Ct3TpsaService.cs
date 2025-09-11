namespace SGP.Api.Services.Ct3Service
{
    public class Ct3TpsaService
    {
        private readonly string _conexionBCT;
        public Ct3TpsaService(IConfiguration configuration)
        {
            _conexionBCT = configuration.GetConnectionString("BCT_TP") ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'BCT_TP' not found.");
        }
    }
}
