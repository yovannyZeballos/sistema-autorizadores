﻿using System;

namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class ProcesoParametro
	{
        public decimal CodProceso { get; set; }
        public string CodParametro { get; set; }
        public string TipParametro { get; set; }
        public string NomParametro { get; set; }
        public string ValParametro { get; set; }
        public string IndActivo { get; set; }
	}
}