using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Globalization;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using System.Web;

namespace SPSA.Autorizadores.Web.Areas.Locales.Controllers
{
    public class InventarioServidorController : Controller
    {

		private readonly IMediator _mediator;

		public InventarioServidorController(IMediator mediator)
		{
			_mediator = mediator;
		}


		// GET: Locales/InventarioServidor
		public ActionResult Index()
        {
			object fechaActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			return View(fechaActual);
		}

        [HttpPost]
		public async Task<JsonResult> ListarInventarioTipo(ListarInventarioTipoQuery request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> InsertarInventario(CrearInventarioServidorCommand request)
		{
			request.Usuario = WebSession.Login;
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> ListarVirtuales(ListarInventarioServidorVirtualQuery request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public ActionResult NuevaVirtual(InventarioServidorVirtualDTO model)
		{
			return PartialView("_NuevaVirtual", model);
		}

		[HttpPost]
		public async Task<JsonResult> InsertarInventarioVirtual(CrearInventarioServidorVirtualCommand request)
		{
			request.Usuario = WebSession.Login;
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> ListarServidores(ListarInventarioServidorQuery request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> ObtenerServidor(ObtenerInventarioServidorQuery request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> EliminarVirtual(EliminarInventarioServidorVirtualCommand request)
		{
			var respuesta = await _mediator.Send(request);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> DescargarMaestro(Aplicacion.Features.InventarioServidor.Commands.DescargarMaestroCommand request)
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
				respuesta = await _mediator.Send(new ImportarInventarioServidorCommand { Archivo = archivo, Usuario = WebSession.Login });
			}

			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> DescargarPlantillas()
		{
			var respuesta = await _mediator.Send(new DescargarPlantillasCommand { Carpeta = "Plantilla_Inventario_Servidor" });
			return Json(respuesta);
		}
	}
}