using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Commands
{
	public class AsociarMenusCommand : IRequest<RespuestaComunDTO>
	{
		public string CodSistema { get; set; }
		public string CodPerfil { get; set; }
		public string UsuCreacion { get; set; }
		public List<AsociarMenuDTO> Menus { get; set; }
	}

	public class AsociarMenusHandler : IRequestHandler<AsociarMenusCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger = SerilogClass._log;
		public AsociarMenusHandler()
		{
			_contexto = new BCTContexto();
		}
		public async Task<RespuestaComunDTO> Handle(AsociarMenusCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			try
			{
				var menusEliminar = await _contexto.RepositorioSegPerfilMenu
					.Obtener(x => x.CodPerfil == request.CodPerfil &&
													  x.CodSistema == request.CodSistema)
					.ToListAsync();
				if (menusEliminar.Count > 0)
					_contexto.RepositorioSegPerfilMenu.EliminarRango(menusEliminar);
				if (request.Menus != null)
				{
					var menushijos = request.Menus.Where(x => x.CodMenuPadre != "#" &&
													   x.CodMenuPadre != "" &&
													   x.CodMenuPadre != "0" &&
													   x.CodMenuPadre != null).ToList();
					var menusPadres = menushijos.Select(x => x.CodMenuPadre)
						.Distinct()
						.ToList();
					foreach (var menu in menusPadres)
					{
						_contexto.RepositorioSegPerfilMenu.Agregar(new Seg_PerfilMenu
						{
							CodPerfil = request.CodPerfil,
							CodMenu = menu,
							CodSistema = request.CodSistema,
							FecCreacion = DateTime.Now,
							UsuCreacion = request.UsuCreacion
						});
					}
					foreach (var menu in menushijos)
					{
						_contexto.RepositorioSegPerfilMenu.Agregar(new Seg_PerfilMenu
						{
							CodPerfil = request.CodPerfil,
							CodMenu = menu.CodMenu,
							CodSistema = request.CodSistema,
							FecCreacion = DateTime.Now,
							UsuCreacion = request.UsuCreacion
						});
					}
				}
				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrió un error al asociar los menus";
				_logger.Error(ex, respuesta.Mensaje);
			}
			return respuesta;
		}
	}
}
