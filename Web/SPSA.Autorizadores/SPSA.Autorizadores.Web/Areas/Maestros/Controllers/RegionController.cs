using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class RegionController : Controller
    {
		private readonly IMediator _mediator;

		public RegionController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Maestros/Region
		public ActionResult Index()
        {
            return View();
        }

		/// <summary>
		/// Este método se utiliza para listar las regiones asociadas.
		/// </summary>
		/// <param name="query">La consulta para listar las regiones asociadas.</param>
		/// <returns>Devuelve un resultado JSON de las regiones asociadas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarRegionesAsociadas(ListarRegionesAsociadasQuery query)
		{
			var respose = await _mediator.Send(query);
			return Json(respose);
		}
	}
}