using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Operaciones.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Operaciones.Controllers
{
    public class ReportesController : Controller
    {
		private readonly IMediator _mediator;
		public ReportesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Operaciones/Reportes/DocumentoElectronico
		public ActionResult DocumentoElectronico()
        {
            return View();
        }


		[HttpPost]
		public async Task<JsonResult> ListarDocumentos(ListarDocumentosElectronicosQuery request)
		{
			var response = await _mediator.Send(request);
			return Json(response);
		}

        [HttpPost]
        public async Task<JsonResult> ListarLocalesAsociadasPorEmpresa(ListarLocalesAsociadasPorEmpresaQuery query)
        {
            query.CodEmpresa = WebSession.JerarquiaOrganizacional.CodEmpresa;
            query.CodUsuario = WebSession.Login;
            var respose = await _mediator.Send(query);
            return Json(respose);
        }
    }
}