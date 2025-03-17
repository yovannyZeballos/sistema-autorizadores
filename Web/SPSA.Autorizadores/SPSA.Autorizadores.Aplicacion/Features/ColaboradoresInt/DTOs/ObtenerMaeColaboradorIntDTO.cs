using System;

namespace SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.DTOs
{
    public class ObtenerMaeColaboradorIntDTO
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public string CodigoOfisis { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string NomTrabajador { get; set; }
        public string TipDocIdent { get; set; }
        public string NumDocIdent { get; set; }
        public DateTime FecIngrEmp { get; set; }
        public DateTime? FecCeseTrab { get; set; }
        public string CodPlan { get; set; }
        public string DesPlan { get; set; }
        public string TiSitu { get; set; }
        public string CodPuesTrab { get; set; }
        public string CodSede { get; set; }
        public string CodMotiSepa { get; set; }
        public string IndInterno { get; set; }
        public int Codjerarquia { get; set; }
    }
}
