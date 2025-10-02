using System;
using System.Configuration;

namespace SPSA.Autorizadores.Infraestructura.Utiles
{
    public abstract class CadenasConexion
    {
        public string CadenaConexionSGP
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SGP"].ConnectionString;
            }
        }

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

		public static string CadenaConexionCT2
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

        public static string CadenaConexionCT3_SPSA
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["CT3_SP"].ConnectionString;
            }
        }

		public static string CadenaConexionCT3_SPSA_SGP
		{
			get
			{
                return ConfigurationManager.ConnectionStrings["CT3_SP_SGP"].ConnectionString;
            }
		}

		private static string CadenaConexionDesdeVariableEntorno(string nombreVariable)
		{
			// Obtiene el valor de la variable de entorno
			var cadena = Environment.GetEnvironmentVariable(nombreVariable, EnvironmentVariableTarget.Process);

			// Si no existe en el proceso, busca en el usuario y luego en la máquina
			if (string.IsNullOrEmpty(cadena))
				cadena = Environment.GetEnvironmentVariable(nombreVariable, EnvironmentVariableTarget.User);

			if (string.IsNullOrEmpty(cadena))
				cadena = Environment.GetEnvironmentVariable(nombreVariable, EnvironmentVariableTarget.Machine);

			return cadena;
		}

	}
}
