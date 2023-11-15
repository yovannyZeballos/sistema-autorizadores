using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

		[HttpPost]
		public JsonResult ListarLocalesAsignados()
		{
			var response = new ListarComunDTO<Dictionary<string, object>>
			{
				Ok = true,
				Columnas = new List<string> { "Codigo", "Nombres" },
				Data = new List<Dictionary<string, object>>()
			};

			foreach (var local in WebSession.LocalesAsignadosXEmpresa)
			{
				response.Data.Add(new Dictionary<string, object>
				{
					{ "Codigo", local.Codigo },
					{ "Nombres", local.Nombre }
				});
			}
			return Json(response);
		}
	}
}