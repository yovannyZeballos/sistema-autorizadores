using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries
{
	public class ObtenerCajaInventarioQuery : IRequest<SovosCajaInventarioDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
		public decimal NumPos { get; set; }
	}

	public class ObtenerCajaInventarioHandler : IRequestHandler<ObtenerCajaInventarioQuery, SovosCajaInventarioDTO>
	{
		private readonly IRepositorioSovosInventarioCaja _repositorioSovosInventarioCaja;
		private readonly IMapper _mapper;

		public ObtenerCajaInventarioHandler(IRepositorioSovosInventarioCaja repositorioSovosInventarioCaja, IMapper mapper)
		{
			_repositorioSovosInventarioCaja = repositorioSovosInventarioCaja;
			_mapper = mapper;
		}

		public async Task<SovosCajaInventarioDTO> Handle(ObtenerCajaInventarioQuery request, CancellationToken cancellationToken)
		{
			var sovosCajaInventarioDTO = new SovosCajaInventarioDTO { Ok = true };
			try
			{
				var sovosCajaInventario = await _repositorioSovosInventarioCaja.Obtener(request.CodEmpresa, request.CodFormato, request.CodLocal, request.NumPos);
				sovosCajaInventarioDTO = _mapper.Map<SovosCajaInventarioDTO>(sovosCajaInventario);
				sovosCajaInventarioDTO.Ok = true;
			}
			catch (System.Exception ex)
			{
				sovosCajaInventarioDTO.Ok = false;
				sovosCajaInventarioDTO.Mensaje = ex.Message;
			}

			return sovosCajaInventarioDTO;
		}
	}
}
