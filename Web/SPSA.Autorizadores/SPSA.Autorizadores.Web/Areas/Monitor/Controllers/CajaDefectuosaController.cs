using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
	public class CajaDefectuosaController : Controller
	{
		private readonly IMediator _mediator;

		public CajaDefectuosaController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: CajaDefectuosa
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> ListarMonitor(ListarLocalMonitorQuery request)
		{
			request.Tipo = (int)TipoMonitor.CAJA_DEFECTUOSA;
			var response = await _mediator.Send(request);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> Procesar(ProcesarCajaDefectuosaCommand request)
		{
			var response = await _mediator.Send(request);
			return Json(response);
		}
	}
}