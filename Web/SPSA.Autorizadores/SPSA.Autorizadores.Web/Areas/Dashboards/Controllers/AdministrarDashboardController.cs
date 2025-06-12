using System.IO;
using System.Linq;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Dashboards.Controllers
{
    public class AdministrarDashboardController : Controller
    {
        private readonly string _reportsFolder = "~/Content/reportes";
        private readonly IMediator _mediator;
        private readonly ISGPContexto _contexto;


        public AdministrarDashboardController(IMediator mediator)
        {
            _mediator = mediator;
            _contexto = new SGPContexto();

            var procesoParametro = _contexto.RepositorioProcesoParametro.Obtener(x => x.CodProceso == 36 && x.CodParametro == "01").FirstOrDefault();
            Stimulsoft.Base.StiLicense.LoadFromFile(procesoParametro.ValParametro);
            //string licencia = "C:\\LicenciaStimul\\license.key";
            //Stimulsoft.Base.StiLicense.LoadFromFile(licencia);
        }

        // GET: Dashboards/AdministrarDashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Viewer(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return HttpNotFound();
            var filePath = Server.MapPath(Path.Combine(_reportsFolder, name + ".mrt"));
            if (!System.IO.File.Exists(filePath)) return HttpNotFound();
            ViewBag.ReportName = name;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult GetReport(string name)
        {
            var rpt = StiReport.CreateNewDashboard();
            rpt.Load(Server.MapPath($"{_reportsFolder}/{name}.mrt"));
            return StiMvcViewer.GetReportResult(rpt);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ViewerEvent()
            => StiMvcViewer.ViewerEventResult();
    }
}