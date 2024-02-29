using AutoMapper;
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
using System.Web;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Commands
{
	public class EliminarMenuCommand : IRequest<RespuestaComunDTO>
	{
		public string CodMenu { get; set; }
	}

	public class EliminarMenuHandler : IRequestHandler<EliminarMenuCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger = SerilogClass._log;

		public EliminarMenuHandler()
		{
			_contexto = new BCTContexto();
		}
		public async Task<RespuestaComunDTO> Handle(EliminarMenuCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };

			try
			{
				var existeMenu = await _contexto.RepositorioSegMenu.Existe(x => x.CodMenu == request.CodMenu);
				if (!existeMenu)
				{
					respuesta.Ok = false;
					respuesta.Mensaje = "El menu no existe";
					return respuesta;
				}

				var menu = await _contexto.RepositorioSegMenu.Obtener(x => x.CodMenu == request.CodMenu).FirstOrDefaultAsync();
				_contexto.RepositorioSegMenu.Eliminar(menu);
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al eliminar el menu";
				_logger.Error(ex, respuesta.Mensaje);
			}

			return respuesta;
		}
	}
}
