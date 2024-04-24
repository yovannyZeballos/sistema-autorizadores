using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
    public class ControlBCTController : Controller
    {
		private readonly IMediator _mediator;

		public ControlBCTController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Monitor/ControlBCT
		public ActionResult Index()
        {
            return View();
        }


		[HttpPost]
		public async Task<JsonResult> Procesar(ProcesarControlBCTCommand request)
		{
			request.CodSucursal = 99999;
			var response = await _mediator.Send(request);
			return Json(response);
		}
	}
}