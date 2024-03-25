using System;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class AperturaDTO
    {
        public string CodLocalPMM { get; set; }
        public string NomLocalPMM { get; set; }
        public string CodLocalSAP { get; set; }
        public string NomLocalSAP { get; set; }
        public string CodLocalSAPNew { get; set; }
        public string CodLocalOfiplan { get; set; }
        public string NomLocalOfiplan { get; set; }
        public string Administrador { get; set; }
        public string NumTelefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Ubigeo { get; set; }
        public string CodComercio { get; set; }
        public string CodCentroCosto { get; set; }
        public DateTime FecApertura { get; set; }
        public string TipEstado { get; set; } = "A";
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime FecModifica { get; set; }
    }
}
