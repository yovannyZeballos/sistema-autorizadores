using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Locales.Controllers
{
	public class InventarioCajaController : Controller
	{
		private readonly IMediator _mediator;

		public InventarioCajaController(IMediator mediator)
		{
			_mediator = mediator;
		}



		// GET: Locales/InventarioCaja
		public ActionResult Index()
		{
			object fechaActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			return View(fechaActual);
		}

		[HttpPost]
		public async Task<JsonResult> InsertarInventario(CrearSovosCajaInventarioCommand request)
		{
			request.Usuario = WebSession.Login;
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> ListarCaracteristicas(ListarCaracteristicasCajaQuery request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> ListarInventario(ListarCajaInventarioQuery request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> ObtenerInventario(ObtenerCajaInventarioQuery request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}


		[HttpPost]
		public async Task<JsonResult> DescargarMaestro(Aplicacion.Features.InventarioCaja.Commands.DescargarMaestroCommand request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> ImportarInventario()
		{
			var respuesta = new RespuestaComunExcelDTO();
			foreach (var fileKey in Request.Files)
			{
				HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
				respuesta = await _mediator.Send(new ImportarInventarioCajaCommand { Archivo = archivo, Usuario = WebSession.Login });
			}

			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> DescargarPlantillas()
		{
			var respuesta = await _mediator.Send(new DescargarPlantillasCommand { Carpeta = "Plantilla_Inventario_Caja"});
			return Json(respuesta);
		}
	}
}