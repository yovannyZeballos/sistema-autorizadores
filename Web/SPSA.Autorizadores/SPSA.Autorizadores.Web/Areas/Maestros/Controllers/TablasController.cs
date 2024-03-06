using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.TablasMae.Commands;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class TablasController : Controller
    {
        private readonly IMediator _mediator;

        public TablasController(IMediator mediator)
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

        [HttpPost]
        public async Task<JsonResult> DescargarPlantillas(string nombreCarpeta)
        {
            var respuesta = await _mediator.Send(new DescargarTablaPlantillasCommand { Carpeta = nombreCarpeta });
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarTablaMaestro(Aplicacion.Features.MantenimientoLocales.Commands.DescargarMaestroCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}