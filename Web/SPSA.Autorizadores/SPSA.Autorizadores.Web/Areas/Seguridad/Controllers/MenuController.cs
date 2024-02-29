using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Seguridad.Controllers
{
    public class MenuController : Controller
    {
		private readonly IMediator _mediator;

		/// <summary>
		/// Constructor de la clase MenuController.
		/// </summary>
		/// <param name="mediator">Instancia de IMediator para la comunicación con los manejadores de comandos y consultas.</param>
		public MenuController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Seguridad/Menu
		public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public async Task<JsonResult> ListarMenus(ListarMenuQuery query)
		{
			var response = await _mediator.Send(query);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> CrearMenu(CrearMenuCommand command)
		{
			command.UsuCreacion = WebSession.Login;
			var response = await _mediator.Send(command);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> ActualizarMenu(ActualizarMenuCommand command)
		{
			command.UsuModificacion = WebSession.Login;
			var response = await _mediator.Send(command);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> EliminarMenu(EliminarMenuCommand command)
		{
			var response = await _mediator.Send(command);
			return Json(response);
		}
	}
}