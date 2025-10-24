using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.TiposActivo.Queries;
using SPSA.Autorizadores.Aplicacion.ViewModel;
using SPSA.Autorizadores.Web.Utiles;
using System;
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
        public async Task<ActionResult> CrearFormInvCaja(InvCajaDTO model)
        {
            ListarInvTipoActivoQuery modelTiposActivo = new ListarInvTipoActivoQuery();
            var tiposActivo = await _mediator.Send(modelTiposActivo);

            ObtenerListasInvCajaQuery modelTipos = new ObtenerListasInvCajaQuery();
            modelTipos.CodEmpresa = model.CodEmpresa;
            var tipos = await _mediator.Send(modelTipos);

            var viewModel = new InvCajaViewModel
            {
                InvCaja = model,
                TiposActivo = tiposActivo.Data,
                TiposModelo = tipos.Data.TiposModelo,
                TiposProcesador = tipos.Data.TiposProcesador,
                TiposMemoria = tipos.Data.TiposMemoria,
                TiposSo = tipos.Data.TiposSo,
                TiposVerSo = tipos.Data.TiposVerSo,
                TiposCapDisco = tipos.Data.TiposCapDisco,
                TiposPuertoBalanza = tipos.Data.TiposPuertoBalanza,
            };


            return PartialView("_CrearInvCaja", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditarFormInvCaja(InvCajaDTO model)
        {
            ListarInvTipoActivoQuery modelTiposActivo = new ListarInvTipoActivoQuery();
            var tiposActivo = await _mediator.Send(modelTiposActivo);

            ObtenerListasInvCajaQuery modelTipos = new ObtenerListasInvCajaQuery();
            modelTipos.CodEmpresa = model.CodEmpresa;
            var tipos = await _mediator.Send(modelTipos);

            var viewModel = new InvCajaViewModel
            {
                InvCaja = model,
                TiposActivo = tiposActivo.Data,
                TiposModelo = tipos.Data.TiposModelo,
                TiposProcesador = tipos.Data.TiposProcesador,
                TiposMemoria = tipos.Data.TiposMemoria,
                TiposSo = tipos.Data.TiposSo,
                TiposVerSo = tipos.Data.TiposVerSo,
                TiposCapDisco = tipos.Data.TiposCapDisco,
                TiposPuertoBalanza = tipos.Data.TiposPuertoBalanza,
            };

            return PartialView("_EditarInvCaja", viewModel);
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
        public async Task<JsonResult> EliminarInvCaja(EliminarInvCajaCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarInvCajaPorCaja(EliminarInvCajaPorCajaCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarInvCajaPorLocal(EliminarInvCajaPorLocalCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
		public async Task<JsonResult> Importar()
		{
			var respuesta = new RespuestaComunExcelDTO();
			foreach (var fileKey in Request.Files)
			{
				HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
                respuesta = await _mediator.Send(new ImportarExcelInvCajaCommand
                {
                    Archivo = archivo,
                    Usuario = WebSession.Login,
                    JerarquiaOrganizacional = WebSession.JerarquiaOrganizacional
                });
                //respuesta = await _mediator.Send(new ImportarExcelInventarioCajaCommand 
                //{ 
                //	Archivo = archivo, 
                //	Usuario = WebSession.Login,
                //	JerarquiaOrganizacional = WebSession.JerarquiaOrganizacional
                //});
            }

            return Json(respuesta);
		}

		[HttpGet]
		public async Task<ActionResult> DescargarPlantillas()
		{
			//var respuesta = await _mediator.Send(new DescargarPlantillasCommand { Carpeta = "Plantilla_Inventarios_v2" });
			//return Json(respuesta);

            var r = await _mediator.Send(new DescargarPlantillasCommand { Carpeta = "Plantilla_Inventarios_v2" });
            if (!r.Ok || r.Archivo == null)
                return Content("No se pudo generar el archivo: " + r.Mensaje);

            var bytes = Convert.FromBase64String(r.Archivo);
            return File(bytes, "application/zip", r.NombreArchivo);
        }

        [HttpGet]
        public async Task<ActionResult> DescargarInventarioCaja(string codEmpresa)
        {
            var result = await _mediator.Send(new DescargarInventarioCajaCommand { CodEmpresa = codEmpresa });

            if (!result.Ok || result.Archivo == null)
            {
                return Content("No se pudo generar el archivo: " + result.Mensaje);
            }

            var bytes = Convert.FromBase64String(result.Archivo);

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.NombreArchivo);
        }


    }
}
