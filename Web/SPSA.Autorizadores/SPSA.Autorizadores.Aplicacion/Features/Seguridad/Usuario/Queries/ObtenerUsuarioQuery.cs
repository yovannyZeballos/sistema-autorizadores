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

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Queries
{
	public class ObtenerUsuarioQuery : IRequest<GenericResponseDTO<ObtenerUsuarioDTO>>
	{
		public string CodUsuario { get; set; }
	}

	public class ObtenerUsuarioHandler : IRequestHandler<ObtenerUsuarioQuery, GenericResponseDTO<ObtenerUsuarioDTO>>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		public ObtenerUsuarioHandler(IMapper mapper)
		{
			_contexto = new SGPContexto();
			_mapper = mapper;
			_logger = SerilogClass._log;
		}
		public async Task<GenericResponseDTO<ObtenerUsuarioDTO>> Handle(ObtenerUsuarioQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<ObtenerUsuarioDTO> { Ok = true };

			try
			{
				var usuario = await _contexto.RepositorioSegUsuario.Obtener(x => x.CodUsuario == request.CodUsuario).FirstOrDefaultAsync();
				if (usuario == null)
				{
					response.Ok = false;
					response.Mensaje = "No se encontró el usuario";
					return response;
				}

				response.Data = _mapper.Map<ObtenerUsuarioDTO>(usuario);

			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Error al obtener el usuario";
				_logger.Error(ex, response.Mensaje);
			}

			return response;
		}
	}
}
