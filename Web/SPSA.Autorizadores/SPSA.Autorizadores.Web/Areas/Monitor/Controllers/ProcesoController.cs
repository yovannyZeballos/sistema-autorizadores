using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
    public class ProcesoController : Controller
    {
		private readonly IMediator _mediator;

		public ProcesoController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Monitor/Proceso
		public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public async Task<JsonResult> ListarProcesos()
		{
			var response = await _mediator.Send(new ListarProcesosQuery());
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> EjecutarProceso(int codProceso)
		{
			var respuestaDefecto = new ListarComunDTO<ListarProcesoDTO> { Ok = false, Mensaje = "Proceso no implementado" };

			switch ((Proceso)codProceso)
			{
				case Proceso.ACTUALIZACION_ESTADO_CIERRE:
					var response = await _mediator.Send(new ProcesarActualizacionEstadoCierreCommand());
					return Json(response);
				//Implementar otros procesos
				default:
					return Json(respuestaDefecto);
			}

		}
	}
}