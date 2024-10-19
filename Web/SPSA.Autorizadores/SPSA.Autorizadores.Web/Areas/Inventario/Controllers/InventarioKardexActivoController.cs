using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class InventarioKardexActivoController : Controller
    {
        private readonly IMediator _mediator;

        public InventarioKardexActivoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/InventarioKardexActivo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CrearFormInvKardexActivo(InvKardexActivoDTO model)
        {
            //ListarInvKardexActivoQuery modelTiposActivo = new ListarInvKardexActivoQuery();
            //var tiposActivo = await _mediator.Send(modelTiposActivo);

            //var viewModel = new InvKardexViewModel
            //{
            //    InvKardex = model,
            //    Activos = tiposActivo.Data
            //};

            return PartialView("_CrearInvKardexActivo", model);
        }

        [HttpPost]
        public async Task<ActionResult> EditarFormInvKardexActivo(InvKardexActivoDTO model)
        {
            //ListarInvKardexActivoQuery modelTiposActivo = new ListarInvKardexActivoQuery();
            //var tiposActivo = await _mediator.Send(modelTiposActivo);

            //var viewModel = new InvKardexViewModel
            //{
            //    InvKardex = model,
            //    Activos = tiposActivo.Data
            //};

            return PartialView("_EditarInvKardexActivo", model);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerInvKardexActivo(ObtenerInvKardexActivoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarInvKardexActivo(ListarInvKardexActivoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> CrearInvKardexActivo(CrearInvKardexActivoCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarInvKardexActivo(ActualizarInvKardexActivoCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarInvkardexActivo(EliminarInvKardexActivoCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}