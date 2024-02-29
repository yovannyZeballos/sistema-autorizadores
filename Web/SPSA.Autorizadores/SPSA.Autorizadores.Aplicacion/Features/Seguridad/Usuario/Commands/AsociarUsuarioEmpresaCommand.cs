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
	/// Comando para asociar un usuario con una empresa.
	/// </summary>
	public class AsociarUsuarioEmpresaCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código de usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Empresas asociadas.
		/// </summary>
		public string[] EmpresasAsociadas { get; set; }
	}

	/// <summary>
	/// Manejador para el comando AsociarUsuarioEmpresaCommand.
	/// </summary>
	public class AsociarUsuarioEmpresaHandler : IRequestHandler<AsociarUsuarioEmpresaCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para el manejador AsociarUsuarioEmpresaHandler.
		/// </summary>
		public AsociarUsuarioEmpresaHandler()
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioEmpresaCommand.
		/// </summary>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioEmpresaCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
			try
			{
				var empresasAsocidas = await _contexto.RepositorioSegEmpresa.Obtener(x => x.CodUsuario == request.CodUsuario).ToListAsync();
				if (empresasAsocidas.Count > 0)
				{
					_contexto.RepositorioSegEmpresa.EliminarRango(empresasAsocidas);
				}

				if (request.EmpresasAsociadas != null)
				{
					foreach (var empresa in request.EmpresasAsociadas)
					{
						_contexto.RepositorioSegEmpresa.Agregar(new Seg_Empresa
						{
							CodUsuario = request.CodUsuario,
							CodEmpresa = empresa
						});
					}
				}


				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar las empresas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
