using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cadenas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cadenas.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaeCadenaController : Controller
    {
		private readonly IMediator _mediator;

		public MaeCadenaController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Maestros/Cadena
		public ActionResult Index()
        {
            return View();
        }

		/// <summary>
		/// Este método se utiliza para listar las cadenas asociadas.
		/// </summary>
		/// <param name="query">La consulta para listar las cadenas asociadas.</param>
		/// <returns>Devuelve un resultado JSON de las cadenas asociadas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarCadenasAsociadas(ListarCadenasAsociadasQuery query)
		{
			var respose = await _mediator.Send(query);
			return Json(respose);
		}

        [HttpPost]
        public async Task<JsonResult> ObtenerCadena(ObtenerMaeCadenaQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ListarCadena(ListarMaeCadenaQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> CrearCadena(CrearMaeCadenaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarCadena(ActualizarMaeCadenaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarExcelCadena(HttpPostedFileBase archivoExcel)
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
                var command = new ImportarMaeCadenaCommand { ArchivoExcel = archivoExcel.InputStream };
                var response = await _mediator.Send(command);

                return Json(response);
            }
        }
    }
}