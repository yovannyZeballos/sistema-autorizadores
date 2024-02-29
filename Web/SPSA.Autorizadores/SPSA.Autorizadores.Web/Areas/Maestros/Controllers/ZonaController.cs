using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Queries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class ZonaController : Controller
    {
		private readonly IMediator _mediator;

		public ZonaController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Maestros/Zona
		public ActionResult Index()
        {
            return View();
        }

		/// <summary>
		/// Este método se utiliza para listar las zonas asociadas.
		/// </summary>
		/// <param name="query">La consulta para listar las zonas asociadas.</param>
		/// <returns>Devuelve un resultado JSON de las zonas asociadas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarZonasAsociadas(ListarZonasAsociadasQuery query)
		{
			var respose = await _mediator.Send(query);
			return Json(respose);
		}
	}
}