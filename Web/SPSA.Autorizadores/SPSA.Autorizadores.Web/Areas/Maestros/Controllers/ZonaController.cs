using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Commands;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class ZonaController : Controller
    {
		private readonly IMediator _mediator;

		public ZonaController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Maestros/Zona
		public ActionResult Index()
        {
            return View();
        }

		/// <summary>
		/// Este método se utiliza para listar las zonas asociadas.
		/// </summary>
		/// <param name="query">La consulta para listar las zonas asociadas.</param>
		/// <returns>Devuelve un resultado JSON de las zonas asociadas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarZonasAsociadas(ListarZonasAsociadasQuery query)
		{
			var respose = await _mediator.Send(query);
			return Json(respose);
		}

        [HttpPost]
        public async Task<JsonResult> ObtenerZona(ObtenerMaeZonaQuery request)
        {
            var respose = await _mediator.Send(request);
            return Json(respose);
        }

        [HttpPost]
        public async Task<JsonResult> ListarZona(ListarMaeZonaQuery request)
        {
            var respose = await _mediator.Send(request);
            return Json(respose);
        }

        [HttpPost]
        public async Task<JsonResult> CrearZona(CrearMaeZonaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarZona(ActualizarMaeZonaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarExcelZona(HttpPostedFileBase archivoExcel)
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
                var command = new ImportarMaeZonaCommand { ArchivoExcel = archivoExcel.InputStream };
                var response = await _mediator.Send(command);

                return Json(response);
            }
        }
    }
}