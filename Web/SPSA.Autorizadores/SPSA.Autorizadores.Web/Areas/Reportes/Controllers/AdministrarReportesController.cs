using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Reportes.Queries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Reportes.Controllers
{
    public class AdministrarReportesController : Controller
    {
        private readonly IMediator _mediator;

        public AdministrarReportesController(IMediator mediator)
        {
            _mediator = mediator;

        }

        // GET: Reportes/AdministrarReportes
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarLocalesCambioPrecio(ListarLocalesCambioPrecioQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarLocalesNotaCredito(ListarLocalesNotaCreditoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}