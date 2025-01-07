using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
	public class ReporteCierreController : Controller
	{
		private readonly IMediator _mediator;

		public ReporteCierreController(IMediator mediator)
		{
			_mediator = mediator;
		}

		public ActionResult Index()
		{
			return View();
		}
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Resumen()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> ReportePivotCierre(ReporteCierrePivotQuery request)
		{
			var response = await _mediator.Send(request);
			var json = Json(response);
			json.MaxJsonLength = int.MaxValue;
			return json;
		}

		[HttpPost]
		public async Task<JsonResult> ReporteCierre(ReporteCierreQuery request)
		{
			var response = await _mediator.Send(request);
			var json = Json(response);
			json.MaxJsonLength = int.MaxValue;
			return json;
		}

		[HttpPost]
		public async Task<JsonResult> ReporteCierreResumen(ReporteCierreResumenQuery request)
		{
			var response = await _mediator.Send(request);
			var json = Json(response);
			json.MaxJsonLength = int.MaxValue;
			return json;
		}
	}
}