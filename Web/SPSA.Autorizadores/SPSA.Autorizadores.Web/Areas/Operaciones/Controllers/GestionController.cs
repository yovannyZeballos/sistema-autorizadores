using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Operaciones.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Operaciones.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Operaciones.Controllers
{
    public class GestionController : Controller
    {
		private readonly IMediator _mediator;

		public GestionController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Operaciones/Gestion/ClienteCen
		public ActionResult ClienteCen()
        {
            return View();
        }

		[HttpPost]
		public async Task<JsonResult> ConsultarClienteCen(ConsultarClienteCenQuery request)
		{
			var response = await _mediator.Send(request);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> InsertarClienteCen(InsertarClienteCenCommand request)
		{
			var response = await _mediator.Send(request);
			return Json(response);
		}
	}
}