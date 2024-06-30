using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.TiposActivo.Queries;
using SPSA.Autorizadores.Aplicacion.ViewModel;
using SPSA.Autorizadores.Web.Utiles;
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
        public ActionResult EditarFormInvCaja(InvCajaDTO model)
        {
            return PartialView("_EditarInvCaja", model);
        }

        [HttpPost]
        public async Task<ActionResult> CrearFormInvCaja(InvCajaDTO model)
        {
            ListarInvTipoActivoQuery modelTiposActivo = new ListarInvTipoActivoQuery();
            var tiposActivo = await _mediator.Send(modelTiposActivo);

            var viewModel = new InvCajaViewModel
            {
                InvCaja = model,
                TiposActivo = tiposActivo.Data
            };


            return PartialView("_CrearInvCaja", viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerInvCaja(ObtenerInvCajaQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarCajas(ListarInvCajaQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> CrearInvCaja(CrearInvCajaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarInvCaja(ActualizarInvCajaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
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
