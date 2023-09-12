using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class SovosLocal
    {
        public string CodEmpresa { get; private set; }
        public string CodLocal { get; private set; }
        public string CodFormato { get; private set; }
        public string NomLocal { get; private set; }
        public string Ip { get; private set; }
        public string IpMascara { get; private set; }
        public string SO { get; private set; }
        public decimal? Grupo { get; private set; }
        public string Estado { get; private set; }
        public string TipoLocal { get; private set; }
        public string IndFactura { get; private set; }
        public string CodigoSunat { get; private set; }
        public string Usuario { get; private set; }
        public DateTime? Fecha { get; private set; }

        public SovosLocal(string codEmpresa, string codLocal, string codFormato, string nomLocal, string ip, string ipMascara, string sO, decimal? grupo, 
            string estado, string tipoLocal, string indFactura, string codigoSunat, string usuario, DateTime? fecha)
        {
            CodEmpresa = codEmpresa;
            CodLocal = codLocal;
            CodFormato = codFormato;
            NomLocal = nomLocal;
            Ip = ip;
            IpMascara = ipMascara;
            SO = sO;
            Grupo = grupo;
            Estado = estado;
            TipoLocal = tipoLocal;
            IndFactura = indFactura;
            CodigoSunat = codigoSunat;
            Usuario = usuario;
            Fecha = fecha;
        }

        public SovosLocal(string codEmpresa, string codLocal, string codFormato, string ip)
        {
            CodEmpresa = codEmpresa;
            CodLocal = codLocal;
            CodFormato = codFormato;
            Ip = ip;
        }

		public SovosLocal(string codEmpresa, string codLocal, string nomLocal)
		{
			CodEmpresa = codEmpresa;
			CodLocal = codLocal;
			NomLocal = nomLocal;
		}
	}
}
