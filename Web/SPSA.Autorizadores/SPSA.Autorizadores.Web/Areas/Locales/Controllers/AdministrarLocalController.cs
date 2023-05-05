using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajaes.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Locales.Controllers
{
    public class AdministrarLocalController : Controller
    {
        private readonly IMediator _mediator;

        public AdministrarLocalController(IMediator mediator)
        {
            _mediator = mediator;
        }


        // GET: Locales/AdministrarLocal
        public ActionResult Index()
        {
            object fechaActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return View(fechaActual);
        }

        [HttpPost]
        public async Task<JsonResult> ListarEmpresas()
        {
            var respuesta = new ListarEmpresaResponse();

            try
            {
                var empresas = await _mediator.Send(new ListarEmpresasQuery());
                respuesta.Ok = true;
                respuesta.Empresas = empresas;
            }
            catch (System.Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarFormatos(ListarFormatosQuery request)
        {
            var respuesta = new ListarFormatoResponse();

            try
            {
                var formatos = await _mediator.Send(request);
                respuesta.Ok = true;
                respuesta.Formatos = formatos;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> CrearLocal(CrearSovosLocalCommand request)
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                respuesta = await _mediator.Send(request);
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarCajas(ListarCajasPorLocalQuery request)
        {
            var respuesta = new ListarCajasPorLocalDTO();

            try
            {
                respuesta = await _mediator.Send(request);
                respuesta.Ok = true;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public ActionResult NuevaCaja(SovosCajaDTO model)
        {
            return PartialView("_NuevaCaja", model);
        }

        [HttpPost]
        public async Task<JsonResult> CrearCaja(CrearSovosCajaCommand request)
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                respuesta = await _mediator.Send(request);
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarLocales(ListarLocalesPorEmpresaQuery request)
        {
            var respuesta = new ListarLocalesPorEmpresaDTO();

            try
            {
                respuesta = await _mediator.Send(request);
                respuesta.Ok = true;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerLocal(ObtenerLocalQuery request)
        {
            var respuesta = new SovosLocalDTO();

            try
            {
                respuesta = await _mediator.Send(request);
                respuesta.Ok = true;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarCajas(EliminarSovosCajasCommand request)
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                respuesta = await _mediator.Send(request);
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarCajas(string codEmpresa, string codLocal, string codFormato)
        {
            var respuesta = new RespuestaComunExcelDTO();
            foreach (var fileKey in Request.Files)
            {
                HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
                respuesta = await _mediator.Send(new ImportarCajasCommand { Archivo = archivo, CodEmpresa = codEmpresa, CodFormato = codFormato, CodLocal = codLocal });
            }

            return Json(respuesta);
        }

        [HttpPost]
        public JsonResult ObtenerFechaSistema()
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                respuesta.Ok = true;
                respuesta.Mensaje = DateTime.Now.ToString("dd/MM/yyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarMaestro(DescargarMaestroCommand request)
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
                respuesta = await _mediator.Send(new ImportarInventarioCajaCommand { Archivo = archivo });
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarPlantillas()
        {
            var respuesta = await _mediator.Send(new DescargarPlantillasCommand());
            return Json(respuesta);

            //var respuesta = new RespuestaComunExcelDTO();

            //var carpeta = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plantillas");

            //using (MemoryStream zipToOpen = new MemoryStream())
            //{
            //    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
            //    {
            //        ZipArchiveEntry readmeEntry;
            //        DirectoryInfo d = new DirectoryInfo(carpeta);
            //        FileInfo[] Files = d.GetFiles("*");
            //        foreach (FileInfo file in Files)
            //        {
            //            readmeEntry = archive.CreateEntryFromFile(file.FullName, file.Name);
            //        }
            //    }

            //    var zip = Convert.ToBase64String(zipToOpen.ToArray());
            //}

            //return Json(respuesta);
        }
    }
}