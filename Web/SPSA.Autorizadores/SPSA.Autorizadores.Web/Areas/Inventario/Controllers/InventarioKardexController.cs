using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries;
using SPSA.Autorizadores.Aplicacion.ViewModel;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class InventarioKardexController : Controller
    {
        private readonly IMediator _mediator;

        public InventarioKardexController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/InventarioKardex
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CrearFormInvKardex(InvKardexDTO model)
        {
            ListarInvKardexActivoQuery modelTiposActivo = new ListarInvKardexActivoQuery();
            var tiposActivo = await _mediator.Send(modelTiposActivo);

            var viewModel = new InvKardexViewModel
            {
                InvKardex = model,
                Activos = tiposActivo.Data
            };

            return PartialView("_CrearInvKardex", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditarFormInvKardex(InvKardexDTO model)
        {
            ListarInvKardexActivoQuery modelTiposActivo = new ListarInvKardexActivoQuery();
            var tiposActivo = await _mediator.Send(modelTiposActivo);

            var viewModel = new InvKardexViewModel
            {
                InvKardex = model,
                Activos = tiposActivo.Data
            };

            return PartialView("_EditarInvKardex", viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerInvKardex(ObtenerInvKardexQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarInvKardex(ListarInvKardexQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> CrearInvKardex(CrearInvKardexCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarInvKardex(ActualizarInvKardexCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarInvkardex(EliminarInvKardexCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}