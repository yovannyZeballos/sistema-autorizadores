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

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Commands
{
	public class CrearMenuCommand : IRequest<RespuestaComunDTO>
	{
		public string NomMenu { get; set; }
		public string UrlMenu { get; set; }
		public string IconoMenu { get; set; }
		public string CodSistema { get; set; }
		public string CodMenuPadre { get; set; }
		public string UsuCreacion { get; set; }
	}

	public class CrearMenuHandler : IRequestHandler<CrearMenuCommand, RespuestaComunDTO>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;

		public CrearMenuHandler(IMapper mapper)
		{
			_contexto = new SGPContexto();
			_mapper = mapper;
		}
		public async Task<RespuestaComunDTO> Handle(CrearMenuCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };

			try
			{
				var codMenu = await _contexto.RepositorioSegMenu.Obtener().MaxAsync(x => x.CodMenu);
				if (string.IsNullOrEmpty(codMenu))
				{
					codMenu = "01";
				}
				else
				{
					codMenu = (int.Parse(codMenu) + 1).ToString("000");
				}

				var menu = _mapper.Map<Seg_Menu>(request);
				menu.CodMenu = codMenu;
				menu.FecCreacion = DateTime.Now;
				menu.IndActivo = "A";

				_contexto.RepositorioSegMenu.Agregar(menu);
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al crear el menu";
				_logger.Error(ex, respuesta.Mensaje);
			}

			return respuesta;
		}
	}
}
