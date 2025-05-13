using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Queries;

namespace SPSA.Autorizadores.Web.Areas.SolicitudCodComercio.Controllers
{
    public class AdministrarSolicitudComercioController : Controller
    {
        private readonly IMediator _mediator;

        public AdministrarSolicitudComercioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: SolicitudCodComercio/AdministrarSolicitudComercio
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<JsonResult> Obtener(ObtenerMaeColaboradorIntQuery request)
        //{
        //    var respuesta = await _mediator.Send(request);
        //    return Json(respuesta);
        //}

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarSolicitudCComercioCabQuery request)
        {

            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}