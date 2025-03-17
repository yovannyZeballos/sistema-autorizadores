using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Queries;

namespace SPSA.Autorizadores.Web.Areas.SolicitudASR.Controllers
{
    public class SolicitudUsuarioController : Controller
    {
        private readonly IMediator _mediator;

        public SolicitudUsuarioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: SolicitudASR/SolicitudUsuario
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarSolicitudUsuarioQuery request)
        {

            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CrearSolicitud(CrearSolicitudUsuarioCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarSolicitud(EliminarSolicitudUsuarioCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarSolicitudes(DescargarSolicitudesUsuarioCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }
    }
}