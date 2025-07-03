using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.PeriodosMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.PeriodosMdr;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.MdrBinesIzipay.Controllers
{
    public class PeriodosMdrController : Controller
    {
        private readonly IMediator _mediator;

        public PeriodosMdrController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: MdrBinesIzipay/PeriodosMdr
        public ActionResult Index()
        {
            return View();
        }      

        [HttpPost]
        public async Task<JsonResult> Crear(CrearMdrPeriodoCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Editar(ActualizarMdrPeriodoCommand command)
        {
            command.UsuModifica = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Eliminar(EliminarMdrPeriodoCommand command)
        {
            command.UsuElimina = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Listar(ListarMdrPeriodoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarPaginadoMdrPeriodoQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}