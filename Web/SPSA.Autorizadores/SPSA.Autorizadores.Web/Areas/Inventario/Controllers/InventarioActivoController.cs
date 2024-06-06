using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Queries;
using System.Collections.Generic;
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
        public async Task<JsonResult> ImportarExcelInventario(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel is null)
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
                var command = new ImportarInventarioActivoCommand { ArchivoExcel = archivoExcel.InputStream };
                var response = await _mediator.Send(command);

                return Json(response);
            }
        }
    }
}