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
				return ConfigurationManager.ConnectionStrings["BCT_SP"].ConnectionString;
			}
		}

		public string CadenaConexionCT2
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["CT2"].ConnectionString;
			}
		}

		public string CadenaConexionCT2_TPSA
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["CT2_TP"].ConnectionString;
			}
		}

		public string CadenaConexionBCT_TPSA
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["BCT_TP"].ConnectionString;
			}
		}

		public string CadenaConexionCT2_HPSA
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["CT2_HP"].ConnectionString;
			}
		}

		public string CadenaConexionBCT_HPSA
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["BCT_HP"].ConnectionString;
			}
		}

	}
}
