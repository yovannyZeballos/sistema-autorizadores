
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;

namespace SPSA.Autorizadores.Web.Utiles
{
    public static class WebSession
    {
        public static string Login
        {
            get { return Comun.Lee<string>("login"); }
            set { Comun.Escribe("login", value); }
        }

        public static string UserName
        {
            get { return Comun.Lee<string>("userName"); }
            set { Comun.Escribe("userName", value); }
        }

        public static string Local
        {
            get { return Comun.Lee<string>("local"); }
            set { Comun.Escribe("local", value); }
        }

        public static string Ruc
        {
            get { return Comun.Lee<string>("ruc"); }
            set { Comun.Escribe("ruc", value); }
        }

        public static string NombreLocal
        {
            get { return Comun.Lee<string>("nombreLocal"); }
            set { Comun.Escribe("nombreLocal", value); }
        }

        public static string TipoSO
        {
            get { return Comun.Lee<string>("tipoSO"); }
            set { Comun.Escribe("tipoSO", value); }
        }

        public static string LocalOfiplan
        {
            get { return Comun.Lee<string>("codigoOfiplan"); }
            set { Comun.Escribe("codigoOfiplan", value); }
        }
        public static List<PermisoDTO> Permisos
        {
            get { return Comun.Lee<List<PermisoDTO>>("permisos"); }
            set { Comun.Escribe("permisos", value); }
        }


        public static List<LocalDTO> Locales
        {
            get { return Comun.Lee<List<LocalDTO>>("locales"); }
            set { Comun.Escribe("locales", value); }
        }
    }
}