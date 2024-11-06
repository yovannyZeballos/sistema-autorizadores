using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Cadenas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Monitor;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
    public class MonitorArchivosController : Controller
    {
		private readonly IMediator _mediator;

		public MonitorArchivosController(IMediator mediator)
		{
			_mediator = mediator;
		}


		// GET: Monitor/MonitorArchivos
		public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public async Task<JsonResult> ListarEmpresasAsociadas()
		{
			var respose = await _mediator.Send(new ListarEmpresasAsociadasQuery { CodUsuario = WebSession.Login});
			return Json(respose);
		}

		[HttpPost]
		public async Task<JsonResult> ListarCadenasAsociadas(ListarCadenasAsociadasQuery query)
		{
			query.CodUsuario = WebSession.Login;
			var response = await _mediator.Send(query);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> ListarRegionesAsociadas(ListarRegionesAsociadasQuery query)
		{
			query.CodUsuario = WebSession.Login;
			var response = await _mediator.Send(query);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> ListarZonasAsociadas(ListarZonasAsociadasQuery query)
		{
			query.CodUsuario = WebSession.Login;
			var response = await _mediator.Send(query);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> ListarLocalesAsociadas(ListarLocalesAsociadasQuery query)
		{
			query.CodUsuario = WebSession.Login;
			var response = await _mediator.Send(query);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> ObtenerParametros()
		{
			var response = await _mediator.Send(new ListarParametrosMonitorArchivoQuery());
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> Procesar(ProcesarMonitorArchivoscommand command)
		{
			var response = await _mediator.Send(command);
			return Json(response);
		}
	}
}