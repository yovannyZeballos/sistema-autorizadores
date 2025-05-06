using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Correo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Queries;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.SolicitudASR.Controllers
{
    public class AprobacionController : Controller
    {
        private readonly IMediator _mediator;

        public AprobacionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: SolicitudASR/Usuario
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarUsuarios(ListarUsuariosQuery request)
        {
            request.UsuarioLogin = WebSession.Login;
			request.CodEmpresa = WebSession.JerarquiaOrganizacional.CodEmpresa;
			var response = await _mediator.Send(request);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> ListarSolicitudes(ListarSolicitudesSolicitadasQuery request)
		{
			request.UsuarioLogin = WebSession.Login;
			request.CodEmpresa = WebSession.JerarquiaOrganizacional.CodEmpresa;
			var response = await _mediator.Send(request);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> Rechazar(ActualizarMotivoRechazoCommand request)
		{
			request.Estado = "R";
			var response = await _mediator.Send(request);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> Aprobar(AprobarSolicitudCommand request)
		{
			request.UsuAutoriza = WebSession.Login;
			request.UsuCreacion = WebSession.Login;
			request.IndActivo = "S";
			request.FlgEnvio = "N";
			var response = await _mediator.Send(request);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> ListarLocalesAsociadasPorEmpresa(ListarLocalesAsociadasPorEmpresaQuery query)
		{
            query.CodEmpresa = WebSession.JerarquiaOrganizacional.CodEmpresa;
            query.CodUsuario = WebSession.Login;
			var respose = await _mediator.Send(query);
			return Json(respose);
		}
	}
}