using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Controllers
{
	public class LocalController : Controller
	{
		private readonly IMediator _mediator;

		public LocalController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<JsonResult> Listar(ListarLocalesXEmpresaQuery request)
		{
			var response = await _mediator.Send(request);
			return Json(response);
		}
	}
}