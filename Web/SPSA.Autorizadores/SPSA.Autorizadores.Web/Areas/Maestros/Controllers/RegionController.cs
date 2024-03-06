using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Commands;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class RegionController : Controller
    {
		private readonly IMediator _mediator;

		public RegionController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Maestros/Region
		public ActionResult Index()
        {
            return View();
        }

		/// <summary>
		/// Este método se utiliza para listar las regiones asociadas.
		/// </summary>
		/// <param name="query">La consulta para listar las regiones asociadas.</param>
		/// <returns>Devuelve un resultado JSON de las regiones asociadas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarRegionesAsociadas(ListarRegionesAsociadasQuery query)
		{
			var respose = await _mediator.Send(query);
			return Json(respose);
		}
        [HttpPost]
        public async Task<JsonResult> ObtenerRegion(ObtenerMaeRegionQuery request)
        {
            var respose = await _mediator.Send(request);
            return Json(respose);
        }

        [HttpPost]
        public async Task<JsonResult> ListarRegion(ListarMaeRegionQuery request)
        {
            var respose = await _mediator.Send(request);
            return Json(respose);
        }

        [HttpPost]
        public async Task<JsonResult> CrearRegion(CrearMaeRegionCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarRegion(ActualizarMaeRegionCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarExcelRegion(HttpPostedFileBase archivoExcel)
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
                var command = new ImportarMaeRegionCommand { ArchivoExcel = archivoExcel.InputStream };
                var response = await _mediator.Send(command);

                return Json(response);
            }
        }

    }
}