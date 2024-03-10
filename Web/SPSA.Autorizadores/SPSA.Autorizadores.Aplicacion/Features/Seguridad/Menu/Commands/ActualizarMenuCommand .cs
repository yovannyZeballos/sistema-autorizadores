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
	public class ActualizarMenuCommand : IRequest<RespuestaComunDTO>
	{
		public string CodMenu { get; set; }
		public string UrlMenu { get; set; }
		public string IconoMenu { get; set; }
		public string NomMenu { get; set; }
		public string UsuModificacion { get; set; }
	}

	public class ActualizarMenuHandler : IRequestHandler<ActualizarMenuCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;

		public ActualizarMenuHandler(IMapper mapper)
		{
			_contexto = new BCTContexto();
			_mapper = mapper;
		}
		public async Task<RespuestaComunDTO> Handle(ActualizarMenuCommand request, CancellationToken cancellationToken)
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

				_mapper.Map(request, menu);
				menu.FecModifica = DateTime.Now;
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al actualizar el menu";
				_logger.Error(ex, respuesta.Mensaje);
			}

			return respuesta;
		}
	}
}
