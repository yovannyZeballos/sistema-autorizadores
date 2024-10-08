﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Inv_Activo
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public string CodActivo { get; set; }
        public string CodModelo { get; set; }
        public string CodSerie { get; set; }
        public string NomMarca { get; set; }
        public string Ip { get; set; }
        public string NomArea { get; set; }
        public string NumOc { get; set; }
        public string NumGuia { get; set; }
        public DateTime? FecSalida{ get; set; }
        public int Antiguedad { get; set; }
        public int Cantidad { get; set; }
        public string IndOperativo { get; set; }
        public string Observacion { get; set; }
        public string Garantia { get; set; }
        public DateTime? FecActualiza { get; set; }

        [ForeignKey("CodActivo")]
        public virtual InvTipoActivo InvTipoActivo { get; set; }

        [ForeignKey("CodEmpresa, CodCadena, CodRegion, CodZona, CodLocal")]
        public virtual Mae_Local MaeLocal { get; set; }

        [ForeignKey("CodEmpresa")]
        public virtual Mae_Empresa MaeEmpresa { get; set; }

    }
}
