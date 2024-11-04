using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class InventarioKardexLocalController : Controller
    {
        private readonly IMediator _mediator;

        public InventarioKardexLocalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/InventarioKardexLocal
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CrearFormInvKardexLocal(InvKardexLocalDTO model)
        {
            return PartialView("_CrearInvKardexLocal", model);
        }

        [HttpPost]
        public async Task<ActionResult> EditarFormInvKardexLocal(InvKardexLocalDTO model)
        {
            return PartialView("_EditarInvKardexLocal", model);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerInvKardexLocal(ObtenerInvKardexLocalQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarInvKardexLocal(ListarInvKardexLocalQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> CrearInvKardexLocal(CrearInvKardexLocalCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarInvKardexLocal(ActualizarInvKardexLocalCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarInvkardexLocal(EliminarInvKardexLocalCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarInvKardexLocal(DescargarInvKardexLocalCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}