using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Producto;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Producto;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IMediator _mediator;

        public ProductosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/Productos
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Crear(CrearMaeProductoCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Editar(ActualizarMaeProductoCommand command)
        {
            command.UsuModifica = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Eliminar(EliminarMaeProductoCommand command)
        {
            command.UsuElimina = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Listar(ListarMaeProductoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarPaginadoMaeProductoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}