﻿using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Mdr_FactorIzipay
    {
        public string CodEmpresa { get; set; }
        public int CodPeriodo { get; set; }
        public string CodOperador { get; set; }
        public string CodClasificacion { get; set; }
        public decimal Factor { get; set; }
        public string IndActivo { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }

        public virtual Mae_Empresa Empresa { get; set; }
        public virtual Mdr_Operador Operador { get; set; }
        public virtual Mdr_ClasificacionPorOperador Clasificacion { get; set; }
    }
}
