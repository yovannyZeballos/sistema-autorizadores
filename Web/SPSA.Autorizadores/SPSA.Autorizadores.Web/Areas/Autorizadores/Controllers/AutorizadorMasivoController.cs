using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Autorizadores.Controllers
{
    public class AutorizadorMasivoController : Controller
    {
		private readonly IMediator _mediator;

		public AutorizadorMasivoController(IMediator mediator)
		{
			_mediator = mediator;
		}


		// GET: Autorizadores/AutorizadorMasivo
		public ActionResult Index()
        {
            return View();
        }

		//[HttpPost]
		//public async Task<JsonResult> AsignarAutorizador(GenerarArchivoPorLocalCommand request)
		//{
		//	var respuesta = await _mediator.Send(request);
		//	return Json(respuesta);
		//}

	}
}