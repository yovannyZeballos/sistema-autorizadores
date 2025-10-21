using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.SerieProducto;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.SerieProducto;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class SeriesProductoController : Controller
    {
        private readonly IMediator _mediator;

        public SeriesProductoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/SeriesProducto
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Crear(CrearMaeSerieProductoCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Editar(ActualizarMaeSerieProductoCommand command)
        {
            command.UsuModifica = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DarBaja(DarBajaSerieProductoCommand command)
        {
            command.UsuEjecucion = WebSession.Login;
            var res = await _mediator.Send(command);
            return Json(res);
        }

        [HttpPost]
        public async Task<JsonResult> Eliminar(EliminarMaeSerieProductoCommand command)
        {
            command.UsuElimina = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Listar(ListarMaeSerieProductoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ListarPorProducto(ListarMaeSeriesPorProductoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ListarPorProductoDisponibles(ListarMaeSeriesPorProductoDisponiblesQuery request)
        {
            request.CodEmpresaOrigen = WebSession.JerarquiaOrganizacional.CodEmpresa;
            request.CodLocalOrigen = WebSession.JerarquiaOrganizacional.CodLocal;

            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarPaginadoMaeSerieProductoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Obtener(ObtenerMaeSerieProductoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}