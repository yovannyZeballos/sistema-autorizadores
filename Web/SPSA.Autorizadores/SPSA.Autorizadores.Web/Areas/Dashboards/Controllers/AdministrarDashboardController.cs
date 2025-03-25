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

        public ActionResult CierreTesoreria()
        {
            return View();
        }

        public ActionResult CierreTesoreriaSpsa()
        {
            return View();
        }

        public ActionResult InventarioLocal()
        {
            return View();
        }

        public ActionResult GetReportCierreTesoreria()
        {
            var report = StiReport.CreateNewDashboard();
            var path = Server.MapPath("~/Content/reportes/cierre-cuadratura.mrt");
            report.Load(path);

            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult GetReportCierreTesoreriaSpsa()
        {
            var report = StiReport.CreateNewDashboard();
            var path = Server.MapPath("~/Content/reportes/cierre-cuadratura-spsa.mrt");
            report.Load(path);

            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult GetReportInvLocal()
        {
            var report = StiReport.CreateNewDashboard();
            var path = Server.MapPath("~/Content/reportes/inventario-local.mrt");
            report.Load(path);

            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }
    }
}