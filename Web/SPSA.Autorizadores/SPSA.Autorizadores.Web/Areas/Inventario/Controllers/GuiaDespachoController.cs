using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.GuiaDespacho;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.GuiaDespacho;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class GuiaDespachoController : Controller
    {
        private readonly IMediator _mediator;

        public GuiaDespachoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/GuiaDespacho
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConfirmacionDespacho()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Registrar(RegistrarGuiaDespachoCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            command.Cabecera.CodEmpresaOrigen = WebSession.JerarquiaOrganizacional.CodEmpresa;
            command.Cabecera.CodLocalOrigen = WebSession.JerarquiaOrganizacional.CodLocal;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ConfirmarEnDestino(ConfirmarDespachoEnDestinoCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarPaginadoGuiaDespachoQuery request)
        {
            request.CodEmpresaOrigen= WebSession.JerarquiaOrganizacional.CodEmpresa;
            request.CodLocalOrigen = WebSession.JerarquiaOrganizacional.CodLocal;
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginadoConfirmacion(ListarPaginadoGuiaDespachoQuery request)
        {
            request.CodEmpresaOrigen = null;
            request.CodLocalOrigen = null;
            request.CodEmpresaDestino = WebSession.JerarquiaOrganizacional.CodEmpresa;
            request.CodLocalDestino= WebSession.JerarquiaOrganizacional.CodLocal;
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}