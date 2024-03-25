using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class InventarioCajaController : Controller
    {
		private readonly IMediator _mediator;

		public InventarioCajaController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Inventario/InventarioCaja
		public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public async Task<JsonResult> Importar()
		{
			var respuesta = new RespuestaComunExcelDTO();
			foreach (var fileKey in Request.Files)
			{
				HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
				respuesta = await _mediator.Send(new ImportarExcelInventarioCajaCommand 
				{ 
					Archivo = archivo, 
					Usuario = WebSession.Login,
					JerarquiaOrganizacional = WebSession.JerarquiaOrganizacional
				});
			}

			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> DescargarPlantillas()
		{
			var respuesta = await _mediator.Send(new DescargarPlantillasCommand { Carpeta = "Plantilla_Inventarios" });
			return Json(respuesta);
		}
	}
}
