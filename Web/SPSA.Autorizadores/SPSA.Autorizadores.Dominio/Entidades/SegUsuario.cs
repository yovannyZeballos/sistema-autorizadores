using System.Collections.Generic;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class SegUsuario
    {
        public SegUsuario()
        {
            Permisos = new List<SegPermiso>();
        }

        public int CodigoSistema { get; set; }
        public string Version { get; set; }
        public string UserBd { get; set; }
        public string PassBd { get; set; }
        public string InstBd { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string CodEmp { get; set; }
        public string UsuNombre { get; set; }
        public List<SegPermiso> Permisos { get; set; }
    }
}
