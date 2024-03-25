using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Ubigeos.Queries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Ubigeos.Controllers
{
    public class UbigeoController : Controller
    {
        private readonly IMediator _mediator;

        public UbigeoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Ubigeos/Ubigeo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarDepartamentos(ListarUbiDepartamentoQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ListarProvincias(ListarUbiProvinciaQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ListarDistritos(ListarUbiDistritoQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerUbigeo(ObtenerUbigeoQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }
    }
}