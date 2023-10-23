using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Cajeros.Controllers
{
	public class ReportesController : Controller
	{
		private readonly IMediator _mediator;

		public ReportesController(IMediator mediator)
		{
			_mediator = mediator;
		}


		// GET: Cajeros/Reportes/Sobres
		public ActionResult Sobres()
		{
			return View();
		}

		// GET: Cajeros/Reportes/DiferenciaCajas
		public ActionResult DiferenciaCajas()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> ReporteSobres(ReporteSobresQuery request)
		{
			var response = await _mediator.Send(request);
			var json = Json(response);
			json.MaxJsonLength = int.MaxValue;
			return json;
		}

		[HttpPost]
		public async Task<JsonResult> ReporteDiferencias(ReporteDiferenciaCajaQuery request)
		{
			var response = await _mediator.Send(request);
			var json = Json(response);
			json.MaxJsonLength = int.MaxValue;
			return json;
		}

		[HttpPost]
		public async Task<JsonResult> ReporteDiferenciasExcel(ReporteDiferenciaCajaExcelCommand request)
		{
			var response = await _mediator.Send(request);
			var json = Json(response);
			json.MaxJsonLength = int.MaxValue;
			return json;
		}

	}
}