using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using SPSA.Autorizadores.Aplicacion.DTO;
using System.Data.Entity;
using SPSA.Autorizadores.Aplicacion.Logger;
using Serilog;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Queries
{
	/// <summary>
	/// Clase de consulta para obtener un sistema.
	/// </summary>
	public class ObtenerSistemaQuery : IRequest<GenericResponseDTO<ObtenerSistemaDTO>>
	{
		/// <summary>
		/// El código del sistema a obtener.
		/// </summary>
		public string CodSistema { get; set; }
	}

	/// <summary>
	/// Manejador de la consulta para obtener un sistema.
	/// </summary>
	public class ObtenerSistemaHandler : IRequestHandler<ObtenerSistemaQuery, GenericResponseDTO<ObtenerSistemaDTO>>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para inyectar las dependencias.
		/// </summary>
		public ObtenerSistemaHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new SGPContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja la consulta para obtener un sistema.
		/// </summary>
		/// <param name="request">La consulta para obtener un sistema.</param>
		/// <param name="cancellationToken">El token de cancelación.</param>
		/// <returns>La respuesta con los datos del sistema.</returns>
		public async Task<GenericResponseDTO<ObtenerSistemaDTO>> Handle(ObtenerSistemaQuery request, CancellationToken cancellationToken)
		{
			var respuesta = new GenericResponseDTO<ObtenerSistemaDTO> { Ok = true };
			try
			{
				var sistema = await _contexto.RepositorioSegSistema.Obtener(s => s.CodSistema == request.CodSistema).FirstOrDefaultAsync();
				if (sistema == null)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "El sistema no existe";
					return respuesta;
				}

				var sistemaDto = _mapper.Map<ObtenerSistemaDTO>(sistema);
				respuesta.Data = sistemaDto;
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al obtener el sistema";
				_logger.Error(ex, "Ocurrió un error al obtener el sistema");
			}
			return respuesta;
		}
	}
}