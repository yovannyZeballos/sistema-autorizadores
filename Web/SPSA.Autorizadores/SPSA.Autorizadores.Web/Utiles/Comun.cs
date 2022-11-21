using System.Web;

namespace SPSA.Autorizadores.Web.Utiles
{
    public static class Comun
    {
        public static void Escribe(string variable, object valor)
        {
            HttpContext.Current.Session[variable] = valor;
        }

        public static T Lee<T>(string variable) 
        {
            object valor = HttpContext.Current.Session[variable];
            if(valor == null) return default;
            return (T)valor;
        }
    }
}