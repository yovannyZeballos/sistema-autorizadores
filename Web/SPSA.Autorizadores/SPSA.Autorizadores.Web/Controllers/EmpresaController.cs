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
			var respuesta = new ListarEmpresaResponse();

			try
			{
				var empresas = await _mediator.Send(new ListarEmpresasOfiplanQuery());
				respuesta.Ok = true;
				respuesta.Empresas = empresas;
			}
			catch (System.Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return Json(respuesta, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public async Task<JsonResult> Listar()
		{
			var respose = await _mediator.Send(new ListarEmpresasQuery());
			return Json(respose);
		}
	}
}