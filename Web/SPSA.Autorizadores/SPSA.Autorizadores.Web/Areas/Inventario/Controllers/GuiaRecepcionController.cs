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
            command.UsuCreacion = WebSession.Login;
            command.Cabecera.CodEmpresaDestino = WebSession.JerarquiaOrganizacional.CodEmpresa;
            command.Cabecera.CodLocalDestino = WebSession.JerarquiaOrganizacional.CodLocal;

            if (!string.IsNullOrEmpty(command.Cabecera.ProveedorRuc)) 
            {
                command.Cabecera.CodEmpresaOrigen = WebSession.JerarquiaOrganizacional.CodEmpresa;
                command.Cabecera.CodLocalOrigen = WebSession.JerarquiaOrganizacional.CodLocal;
            }

            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

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