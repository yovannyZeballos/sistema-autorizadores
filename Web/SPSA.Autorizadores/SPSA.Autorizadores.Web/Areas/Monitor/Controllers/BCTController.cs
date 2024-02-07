using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
	public class BCTController : Controller
	{
		private readonly IMediator _mediator;

		public BCTController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Monitor/BCT
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> Listar(ObtenerRegistrosMonitorBCTQuery request)
		{
			var response = await _mediator.Send(request);
			return Json(response);
		}


		[HttpPost]
		public async Task<JsonResult> Procesar(ProcesarMonitorBCTCommand request)
		{
			var response = await _mediator.Send(request);
			return Json(response);
		}


		[HttpPost]
		public async Task<JsonResult> Parametros()
		{
			var response = await _mediator.Send(new ObtenerParametrosQuery());
			return Json(response);
		}
	}
}