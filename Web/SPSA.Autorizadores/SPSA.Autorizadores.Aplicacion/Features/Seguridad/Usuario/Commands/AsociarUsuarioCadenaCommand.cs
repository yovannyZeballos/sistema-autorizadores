using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands
{
	/// <summary>
	/// Comando para asociar un usuario a una cadena.
	/// </summary>
	public class AsociarUsuarioCadenaCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código del usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Cadenas a asociar al usuario.
		/// </summary>
		public string[] CadenasAsociadas { get; set; }
	}

	/// <summary>
	/// Manejador del comando AsociarUsuarioCadenaCommand.
	/// </summary>
	public class AsociarUsuarioCadenaHandler : IRequestHandler<AsociarUsuarioCadenaCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor del manejador.
		/// </summary>
		public AsociarUsuarioCadenaHandler()
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioCadenaCommand.
		/// </summary>
		/// <param name="request">El comando.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Una tarea que representa la operación asincrónica. El resultado de la tarea es una RespuestaComunDTO.</returns>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioCadenaCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
			try
			{
				var cadenasAsocidas = await _contexto.RepositorioSegCadena
					.Obtener(x => x.CodUsuario == request.CodUsuario && x.CodEmpresa == request.CodEmpresa)
					.ToListAsync();

				if (cadenasAsocidas.Count > 0)
				{
					_contexto.RepositorioSegCadena.EliminarRango(cadenasAsocidas);
				}

				if (request.CadenasAsociadas != null)
				{
					foreach (var cadena in request.CadenasAsociadas)
					{
						_contexto.RepositorioSegCadena.Agregar(new Seg_Cadena
						{
							CodUsuario = request.CodUsuario,
							CodEmpresa = request.CodEmpresa,
							CodCadena = cadena
						});
					}
				}


				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar las cadenas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}