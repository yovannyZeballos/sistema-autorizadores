using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.TablasMae.Commands;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaeTablasController : Controller
    {
        private readonly IMediator _mediator;

        public MaeTablasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Maestros/Tablas
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearEditarEmpresa(MaeEmpresaDTO model)
        {
            return PartialView("_CrearEditarEmpresa", model);
        }

        [HttpPost]
        public ActionResult CrearEditarCadena(MaeCadenaDTO model)
        {
            return PartialView("_CrearEditarCadena", model);
        }

        [HttpPost]
        public ActionResult CrearEditarRegion(MaeRegionDTO model)
        {
            return PartialView("_CrearEditarRegion", model);
        }

        [HttpPost]
        public ActionResult CrearEditarZona(MaeZonaDTO model)
        {
            return PartialView("_CrearEditarZona", model);
        }

        [HttpGet]
        public ActionResult DescargarPlantillas(string nomPlantilla) // parámetro opcional si lo necesitas
        {
            // Nombre del archivo que verá el usuario al descargar
            var downloadFileName = nomPlantilla;

            // Ruta física del archivo en el servidor
            var folder = Server.MapPath("~/App_Data/Plantillas");
            var fullPath = Path.Combine(folder, downloadFileName);

            if (!System.IO.File.Exists(fullPath))
                return HttpNotFound("No se encontró la plantilla.");

            // MIME para .xlsx
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // Devuelve como descarga (attachment)
            return File(fullPath, contentType, downloadFileName);

            // Alternativa en memoria:
            // var bytes = System.IO.File.ReadAllBytes(fullPath);
            // return File(bytes, contentType, downloadFileName);

            // Alternativa stream (para archivos grandes):
            // var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            // return new FileStreamResult(stream, contentType) { FileDownloadName = downloadFileName };
        }


        [HttpPost]
        public async Task<JsonResult> DescargarTablaMaestro(Aplicacion.Features.MantenimientoLocales.Commands.DescargarMaestroCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}