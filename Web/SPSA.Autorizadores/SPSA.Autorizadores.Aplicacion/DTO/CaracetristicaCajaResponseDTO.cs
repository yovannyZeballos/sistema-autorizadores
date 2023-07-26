using System.Collections.Generic;

namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public class CaracetristicaCajaResponseDTO : RespuestaComunDTO
	{
        public List<CaracetristicaCajaDTO> CaracetristicasCaja { get; set; }
		public CaracetristicaCajaResponseDTO()
		{
			CaracetristicasCaja = new List<CaracetristicaCajaDTO>();
		}
    }
}
