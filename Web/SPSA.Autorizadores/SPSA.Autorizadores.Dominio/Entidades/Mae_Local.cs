﻿namespace SPSA.Autorizadores.Dominio.Entidades
{
    /// <summary>
    /// Representa la entidad Mae_Local en el dominio.
    /// </summary>
    public class Mae_Local
    {
        /// <summary>
        /// Obtiene o establece el código de la empresa.
        /// </summary>
        public string CodEmpresa { get; set; }

        /// <summary>
        /// Obtiene o establece el código de la cadena.
        /// </summary>
        public string CodCadena { get; set; }

        /// <summary>
        /// Obtiene o establece el código de la región.
        /// </summary>
        public string CodRegion { get; set; }

        /// <summary>
        /// Obtiene o establece el código de la zona.
        /// </summary>
        public string CodZona { get; set; }

        /// <summary>
        /// Obtiene o establece el código del local.
        /// </summary>
        public string CodLocal { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del local.
        /// </summary>
        public string NomLocal { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de estado.
        /// </summary>
        public string TipEstado { get; set; }

        public string CodLocalPMM { get; set; }
        public string CodLocalOfiplan { get; set; }
        public string NomLocalOfiplan { get; set; }
        public string CodLocalSunat { get; set; }
        public string Ip { get; set; }
	}
}