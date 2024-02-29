using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Cadenas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class CadenaController : Controller
    {
		private readonly IMediator _mediator;

		public CadenaController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Maestros/Cadena
		public ActionResult Index()
        {
            return View();
        }

		/// <summary>
		/// Este método se utiliza para listar las cadenas asociadas.
		/// </summary>
		/// <param name="query">La consulta para listar las cadenas asociadas.</param>
		/// <returns>Devuelve un resultado JSON de las cadenas asociadas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarCadenasAsociadas(ListarCadenasAsociadasQuery query)
		{
			var respose = await _mediator.Send(query);
			return Json(respose);
		}
	}
}