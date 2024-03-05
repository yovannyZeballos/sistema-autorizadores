using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Threading.Tasks;
using System.Web.Mvc;
using ListarLocalesQuery = SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Queries.ListarLocalesQuery;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
    public class ReporteTesoreriaController : Controller
    {
        private readonly IMediator _mediator;

        public ReporteTesoreriaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index()
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
        public async Task<JsonResult> ListarEmpresas(ListarEmpresasQuery request)
        {
            request.CodUsuario = WebSession.Login;
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }
        [HttpPost]
        public async Task<JsonResult> ListarCadenas(ListarCadenasQuery request)
        {
            request.CodUsuario = WebSession.Login;
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarRegiones(ListarRegionesQuery request)
        {
            request.CodUsuario = WebSession.Login;
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarZonas(ListarZonasQuery request)
        {
            request.CodUsuario = WebSession.Login;
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarLocales(ListarLocalesQuery request)
        {
            request.CodUsuario = WebSession.Login;
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}