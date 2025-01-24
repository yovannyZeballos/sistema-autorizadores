using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Linq;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
	public class ReporteCierreController : Controller
	{
		private readonly IMediator _mediator;
		private readonly ISGPContexto _contexto;

		public ReporteCierreController(IMediator mediator)
		{
			_mediator = mediator;
            _contexto = new SGPContexto();

			var procesoParametro = _contexto.RepositorioProcesoParametro.Obtener(x => x.CodProceso == 36 && x.CodParametro == "01" ).FirstOrDefault();
            Stimulsoft.Base.StiLicense.LoadFromFile(procesoParametro.ValParametro);
        }

		public ActionResult Index()
		{
			return View();
		}
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult GetReport()
        {
            var report = StiReport.CreateNewDashboard();
            var path = Server.MapPath("~/Content/Reportes/Report.mrt");
            report.Load(path);

            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
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