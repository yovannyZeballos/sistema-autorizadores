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
            var perfilMenuAgregar = new List<Seg_PerfilMenu>();
            var perfilMenuEliminar = new List<Seg_PerfilMenu>();

            try
			{

                var menus = request.Menus.Where(x => x.CodMenu != "0").ToList();

                var perfilMenuAsocidas = await _contexto.RepositorioSegPerfilMenu
                    .Obtener(x => x.CodPerfil == request.CodPerfil
                    && x.CodSistema == request.CodSistema)
                    .ToListAsync();

                HashSet<string> codigosHashSet = new HashSet<string>();
                foreach (var perfilmenu in perfilMenuAsocidas)
                {
                    codigosHashSet.Add(perfilmenu.CodMenu);
                }

                // Identificar los códigos para agregar
                foreach (var menu in menus)
                {
                    if (!codigosHashSet.Contains(menu.CodMenu))
                    {
                        perfilMenuAgregar.Add(new Seg_PerfilMenu
                        {
                            CodPerfil = request.CodPerfil,
                            CodMenu = menu.CodMenu,
                            CodSistema = request.CodSistema,
                            FecCreacion = DateTime.Now,
                            UsuCreacion = request.UsuCreacion
                        });
                    }
                }

                // Convertir la lista de objetos en un array de solo códigos string
                string[] arrayCodMenus = menus.Select(obj => obj.CodMenu).ToArray();

                // Identificar los códigos para eliminaren cascada
                foreach (var codigo in codigosHashSet)
                {
                    if (!Array.Exists(arrayCodMenus, c => c == codigo))
                    {
                        perfilMenuEliminar.Add(new Seg_PerfilMenu
                        {
                            CodPerfil = request.CodPerfil,
                            CodMenu = codigo,
                            CodSistema = request.CodSistema,
                            FecCreacion = DateTime.Now,
                            UsuCreacion = request.UsuCreacion
                        });
                    }
                }

                if (perfilMenuEliminar.Count > 0)
                {
                    foreach (var perfilMenu in perfilMenuEliminar)
                    {
                        var existenHijos = await _contexto.RepositorioSegMenu.Obtener(m => m.CodMenuPadre == perfilMenu.CodMenu).ToListAsync();

                        if (existenHijos.Count == 0)
                        {
                            var perfilmenuDesasociada = await _contexto.RepositorioSegPerfilMenu.Obtener(e => e.CodSistema == perfilMenu.CodSistema && e.CodPerfil == perfilMenu.CodPerfil
                                                                                            && e.CodMenu == perfilMenu.CodMenu).FirstOrDefaultAsync();
                            _contexto.RepositorioSegPerfilMenu.Eliminar(perfilmenuDesasociada);
                        }
                    }
                }

                if (perfilMenuAgregar.Count > 0)
                {
                    foreach (var perfilMenu in perfilMenuAgregar)
                    {
                        _contexto.RepositorioSegPerfilMenu.Agregar(perfilMenu);
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
