using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries
{
	public class ListarCaracteristicasCajaQuery : IRequest<CaracetristicaCajaResponseDTO>
	{
		public string CodEmpresa { get; set; }
		public int Tipo { get; set; }
	}

	public class ListarCaracteristicasCajaHandler : IRequestHandler<ListarCaracteristicasCajaQuery, CaracetristicaCajaResponseDTO>
	{
		private readonly IRepositorioSovosInventarioCaja _repositorioSovosInventarioCaja;

		public ListarCaracteristicasCajaHandler(IRepositorioSovosInventarioCaja repositorioSovosInventarioCaja)
		{
			_repositorioSovosInventarioCaja = repositorioSovosInventarioCaja;
		}

		public async Task<CaracetristicaCajaResponseDTO> Handle(ListarCaracteristicasCajaQuery request, CancellationToken cancellationToken)
		{
			var response = new CaracetristicaCajaResponseDTO { Ok = true };

			try
			{
				var caracteristicas = await _repositorioSovosInventarioCaja.ListarCaracteristicas(request.CodEmpresa, request.Tipo);

				foreach (var item in caracteristicas)
					response.CaracetristicasCaja.Add(new CaracetristicaCajaDTO { Id = item.Id, Descripcion = item.Descripcion });
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}

			return response;
		}
	}
}
