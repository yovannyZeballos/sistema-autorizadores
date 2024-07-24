using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Queries
{
	public class ObtenerMenuQuery : IRequest<GenericResponseDTO<ListarMenuDTO>>
	{
		public string CodMenu { get; set; }
	}

	public class ObtenerMenuHandler : IRequestHandler<ObtenerMenuQuery, GenericResponseDTO<ListarMenuDTO>>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;


		public ObtenerMenuHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new SGPContexto();
		}

		public async Task<GenericResponseDTO<ListarMenuDTO>> Handle(ObtenerMenuQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<ListarMenuDTO> { Ok = true };
			try
			{
				var menu = await _contexto.RepositorioSegMenu.Obtener(x => x.CodMenu == request.CodMenu).FirstOrDefaultAsync();
				response.Data = _mapper.Map<ListarMenuDTO>(menu);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al obtener el menu";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
