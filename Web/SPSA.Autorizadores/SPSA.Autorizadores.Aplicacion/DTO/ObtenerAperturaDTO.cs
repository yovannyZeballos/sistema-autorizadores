using System;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class ObtenerAperturaDTO
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
        public string TipEstado { get; set; }
        public string CodEAN { get; set; }
        public string CodSUNAT { get; set; }
        public string NumGuias { get; set; }
        public string CentroDistribu { get; set; }
        public string TdaEspejo { get; set; }
        public string Mt2Sala { get; set; }
        public string Spaceman { get; set; }
        public string ZonaPricing { get; set; }
        public string Geolocalizacion { get; set; }
        public DateTime? FecCierre { get; set; }
    }
}
