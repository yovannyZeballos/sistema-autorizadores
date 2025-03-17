using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.Queries;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaeColaboradorIntController : Controller
    {
        private readonly IMediator _mediator;

        public MaeColaboradorIntController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Maestros/MaeColaboradorInt
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Obtener(ObtenerMaeColaboradorIntQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarMaeColaboradorIntQuery request)
        {

            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}