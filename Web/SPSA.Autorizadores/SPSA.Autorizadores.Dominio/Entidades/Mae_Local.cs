using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mae_Local
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public string NomLocal { get; set; }
        public string TipEstado { get; set; }
        public string CodLocalPMM { get; set; }
        public string CodLocalOfiplan { get; set; }
        public string NomLocalOfiplan { get; set; }
        public string CodLocalSunat { get; set; }
        public string Ip { get; set; }
        public string DirLocal { get; set; }
        public string Ubigeo { get; set; }
        public string CodLocalAlterno { get; set; }
        public DateTime? FecApertura { get; set; }
        public DateTime? FecCierre { get; set; }
        public DateTime? FecEntrega { get; set; }
	}
}