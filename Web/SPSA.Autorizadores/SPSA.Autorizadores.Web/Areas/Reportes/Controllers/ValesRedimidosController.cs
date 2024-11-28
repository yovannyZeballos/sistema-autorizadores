using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Reportes.Queries;

namespace SPSA.Autorizadores.Web.Areas.Reportes.Controllers
{
    public class ValesRedimidosController : Controller
    {
        private readonly IMediator _mediator;

        public ValesRedimidosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Reportes/ValesRedimidos
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Listar(ListarValesRedimidosQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }
    }
}