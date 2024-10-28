using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Queries;
using SPSA.Autorizadores.Aplicacion.Features.TiposActivo.Queries;
using SPSA.Autorizadores.Aplicacion.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class InventarioActivoController : Controller
    {
        private readonly IMediator _mediator;

        public InventarioActivoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearEditarInvActivo(InvActivoDTO model)
        {
            return PartialView("_CrearEditarActivo", model);
        }

        [HttpPost]
        public async Task<ActionResult> CrearFormInvActivo(InvActivoDTO model)
        {
            ListarInvTipoActivoQuery modelTiposActivo = new ListarInvTipoActivoQuery();
            var tiposActivo = await _mediator.Send(modelTiposActivo);

            var viewModel = new InvActivoViewModel
            {
                InvActivo = model,
                TiposActivo = tiposActivo.Data.Where(x => x.IndEstado == "A").ToList()
            };

            return PartialView("_CrearInvActivo", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditarFormInvActivo(InvActivoDTO model)
        {
            ListarInvTipoActivoQuery modelTiposActivo = new ListarInvTipoActivoQuery();
            var tiposActivo = await _mediator.Send(modelTiposActivo);

            var viewModel = new InvActivoViewModel
            {
                InvActivo = model,
                TiposActivo = tiposActivo.Data.Where(x=> x.IndEstado == "A").ToList()
            };

            return PartialView("_EditarInvActivo", viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerInvActivo(ObtenerInvActivoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarActivos(ListarInvActivoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> CrearInvActivo(CrearInvActivoCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarInvActivo(ActualizarInvActivoCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarExcelInventario()
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
                    var command = new ImportarInventarioActivoCommand { ArchivoExcel = archivo.InputStream };
                    var response = await _mediator.Send(command);

                    return Json(response);
                }
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarInvActivo(DescargarInvActivoCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }
    }
}