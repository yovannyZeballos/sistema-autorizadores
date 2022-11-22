
namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Autorizador : Persona
    {
        public int CodLocal { get; private set; }
        public string UsuarioCreacion { get; private set; }
        public string CodigoAutorizador { get; private set; }
        public string FechaCreacion { get; private set; }
        public string NumeroTarjeta { get; private set; }
        public string Impreso { get; private set; }

        public Autorizador(string codigo, int codLocal, string codigoAutorizador, string numeroTarjeta)
        {
            Codigo = codigo;
            CodLocal = codLocal;
            CodigoAutorizador = codigoAutorizador;
            NumeroTarjeta = numeroTarjeta;
        }

        public Autorizador(string codigo, string nombres, string apellidoPaterno, string apellidoMaterno, string numeroDocumento, int codLocal, string usuarioCreacion)
        {
            Codigo = codigo;
            Nombres = nombres;
            ApellidoPaterno = apellidoPaterno;
            ApellidoMaterno = apellidoMaterno;
            NumeroDocumento = numeroDocumento;
            CodLocal = codLocal;
            UsuarioCreacion = usuarioCreacion;
            Estado = "A";
        }

        public Autorizador(string codigo, string nombres, string apellidoPaterno, string apellidoMaterno, string numeroDocumento, string estado, int codLocal, string usuarioCreacion, string codigoAutorizador, string fechaCreacion, string numeroTarjeta, string impreso)
        {
            Codigo = codigo;
            Nombres = nombres;
            ApellidoPaterno = apellidoPaterno;
            ApellidoMaterno = apellidoMaterno;
            NumeroDocumento = numeroDocumento;
            Estado = estado;
            CodLocal = codLocal;
            UsuarioCreacion = usuarioCreacion;
            CodigoAutorizador = codigoAutorizador;
            FechaCreacion = fechaCreacion;
            NumeroTarjeta = numeroTarjeta;
            Impreso = impreso;
        }

        public Autorizador(string codigo, string usuarioCreacion, string codigoAutorizador)
        {
            Codigo = codigo;
            UsuarioCreacion = usuarioCreacion;
            CodigoAutorizador = codigoAutorizador;
        }
    }
}
