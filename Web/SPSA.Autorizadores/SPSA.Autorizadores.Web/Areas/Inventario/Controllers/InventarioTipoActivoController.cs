using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.TiposActivo.Command;
using SPSA.Autorizadores.Aplicacion.Features.TiposActivo.Queries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class InventarioTipoActivoController : Controller
    {
        private readonly IMediator _mediator;

        public InventarioTipoActivoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/InventarioTipoActivo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarTiposActivo(ListarInvTipoActivoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarInvTiposActivo(DescargarInvTiposActivoCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }
    }
}