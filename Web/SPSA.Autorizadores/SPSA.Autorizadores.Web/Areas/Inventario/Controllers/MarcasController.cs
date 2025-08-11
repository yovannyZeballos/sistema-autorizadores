using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Marca;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Marca;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class MarcasController : Controller
    {
        private readonly IMediator _mediator;

        public MarcasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/Marcas
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Crear(CrearMaeMarcaCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Editar(ActualizarMaeMarcaCommand command)
        {
            command.UsuModifica = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Eliminar(EliminarMaeMarcaCommand command)
        {
            command.UsuElimina = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Listar(ListarMaeMarcaQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarPaginadoMaeMarcaQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}