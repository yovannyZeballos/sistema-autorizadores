using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Kardex;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Kardex;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class KardexController : Controller
    {
        private readonly IMediator _mediator;

        public KardexController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/Kardex
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Movimientos()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Registrar(RegistrarMovKardexCommand command)
        {
            command.Usuario = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarPaginadoMovKardexQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginadoPorProducto(ListarPaginadoMovKardexPorProductoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> DescargarPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            var result = await _mediator.Send(new DescargarMovKardexPorFechasCommand { FechaInicio = fechaInicio, FechaFin = fechaFin });

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