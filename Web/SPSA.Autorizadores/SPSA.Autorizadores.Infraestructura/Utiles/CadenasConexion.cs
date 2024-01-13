using System.Configuration;

namespace SPSA.Autorizadores.Infraestructura.Utiles
{
    public abstract class CadenasConexion
    {
        public string CadenaConexionSeguridad
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Seguridad"].ConnectionString;
            }
        }

        public string CadenaConexionCarteleria
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Carteleria"].ConnectionString;
            }
        }

        public string CadenaConexionAutorizadores
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Autorizadores"].ConnectionString;
            }
        }

		public string CadenaConexionBCT
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["BCT"].ConnectionString;
			}
		}
	}
}
