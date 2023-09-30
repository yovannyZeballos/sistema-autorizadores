using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Controllers
{
	public class EmpresaController : Controller
	{

		private readonly IMediator _mediator;

		public EmpresaController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> ListarEmpresas()
		{
			var respose = await _mediator.Send(new ListarEmpresasOfiplanQuery());
			return Json(respose, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public async Task<JsonResult> ListarOfiplan()
		{
			var respose = await _mediator.Send(new ListarEmpresasOfiplanQuery());
			return Json(respose);
		}

		[HttpPost]
		public async Task<JsonResult> Listar()
		{
			var respose = await _mediator.Send(new ListarEmpresasQuery());
			return Json(respose);
		}
	}
}