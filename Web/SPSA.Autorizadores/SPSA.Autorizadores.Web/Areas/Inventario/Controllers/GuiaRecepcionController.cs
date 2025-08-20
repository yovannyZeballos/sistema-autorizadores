using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.GuiaRecepcion;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.GuiaRecepcion;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Inventario.Controllers
{
    public class GuiaRecepcionController : Controller
    {
        private readonly IMediator _mediator;

        public GuiaRecepcionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Inventario/GuiaRecepcion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Proveedor()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Registrar(RegistrarGuiaRecepcionCommand command)
        {
            // Evita NullReference si el binder no pobló algo:
            if (command == null)
                command = new RegistrarGuiaRecepcionCommand();

            if (command.Cabecera == null)
                command.Cabecera = new GuiaRecepcionCabeceraCommand();

            command.UsuCreacion = WebSession.Login;
            command.Cabecera.CodEmpresaDestino = WebSession.JerarquiaOrganizacional.CodEmpresa;
            command.Cabecera.CodLocalDestino = WebSession.JerarquiaOrganizacional.CodLocal;

            // (Opcional) Si quieres detectar problemas de binding:
            //if (!ModelState.IsValid) return Json(new RespuestaComunDTO { Ok = false, Mensaje = "Datos inválidos." });

            var respuesta = await _mediator.Send(command);
            return Json(respuesta);

            //command.UsuCreacion = WebSession.Login;
            //command.Cabecera.CodEmpresaDestino = WebSession.JerarquiaOrganizacional.CodEmpresa;
            //command.Cabecera.CodLocalDestino = WebSession.JerarquiaOrganizacional.CodLocal;
            //var respuesta = await _mediator.Send(command);
            //return Json(respuesta);
        }

        //[HttpPost]
        //public async Task<JsonResult> Listar(ListarMaeProductoQuery request)
        //{
        //    var respuesta = await _mediator.Send(request);
        //    return Json(respuesta, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarPaginadoGuiaRecepcionQuery request)
        {
            request.CodEmpresaDestino = WebSession.JerarquiaOrganizacional.CodEmpresa;
            request.CodLocalDestino = WebSession.JerarquiaOrganizacional.CodLocal;
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}