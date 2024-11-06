using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries;
using SPSA.Autorizadores.Aplicacion.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
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

            ListarInvKardexLocalQuery modelLocales = new ListarInvKardexLocalQuery();
            var locales = await _mediator.Send(modelLocales);

            var viewModel = new InvKardexViewModel
            {
                InvKardex = model,
                Activos = tiposActivo.Data,
                Locales = locales.Data
            };

            return PartialView("_CrearInvKardex", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditarFormInvKardex(InvKardexDTO model)
        {
            ListarInvKardexActivoQuery modelTiposActivo = new ListarInvKardexActivoQuery();
            var tiposActivo = await _mediator.Send(modelTiposActivo);

            ListarInvKardexLocalQuery modelLocales = new ListarInvKardexLocalQuery();
            var locales = await _mediator.Send(modelLocales);

            var viewModel = new InvKardexViewModel
            {
                InvKardex = model,
                Activos = tiposActivo.Data,
                Locales = locales.Data
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

        [HttpPost]
        public async Task<JsonResult> ImportarExcelInvKardex()
        {
            var respuesta = new RespuestaComunExcelDTO();
            foreach (var fileKey in Request.Files)
            {
                HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
                if (archivo is null)
                {
                    var response = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
                    response.Ok = false;
                    response.Mensaje = "Se encontraron algunos errores en el archivo";
                    response.Errores.Add(new ErroresExcelDTO
                    {
                        Fila = 1,
                        Mensaje = "No se ha seleccionado ningun archivo."
                    });

                    return Json(response);
                }
                else
                {
                    var command = new ImportarInvKardexCommand { ArchivoExcel = archivo.InputStream };
                    var response = await _mediator.Send(command);

                    return Json(response);
                }
            }

            return Json(respuesta);
        }

    }
}