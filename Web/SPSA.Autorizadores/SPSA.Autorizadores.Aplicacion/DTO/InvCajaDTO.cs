using SPSA.Autorizadores.Dominio.Entidades;
using System;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
    public class InvCajaDTO
    {
        public string CodEmpresa { get; set; }

        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public decimal NumCaja { get; set; }
        public string CodActivo { get; set; }
        public string CodModelo { get; set; }
        public string CodSerie { get; set; }
        public string NumAdenda { get; set; }
        public DateTime? FecGarantia { get; set; }
        public string TipEstado { get; set; }
        public string TipProcesador { get; set; }
        public string Memoria { get; set; }
        public string DesSo { get; set; }
        public string VerSo { get; set; }
        public string CapDisco { get; set; }
        public string TipDisco { get; set; }
        public string DesPuertoBalanza { get; set; }
        public string TipoCaja { get; set; }
        public string Hostname { get; set; }
        public DateTime? FechaInicioLising { get; set; }
        public DateTime? FechaFinLising { get; set; }

        public InvTipoActivo InvTipoActivo { get; set; }

    }
}
