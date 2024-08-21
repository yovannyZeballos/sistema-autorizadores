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
	public class DiferenciaTransaccionesController : Controller
	{
		private readonly IMediator _mediator;

		public DiferenciaTransaccionesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Monitor/DiferenciaTransacciones
		public ActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public async Task<JsonResult> Procesar(ProcesarMonitorLocalBCTCommand command)
		{
			var response = await _mediator.Send(command);
			return Json(response);
		}
    }
}