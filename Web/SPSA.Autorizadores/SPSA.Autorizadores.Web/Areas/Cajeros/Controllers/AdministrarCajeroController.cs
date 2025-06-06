﻿using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Cajeros.Controllers
{
	public class AdministrarCajeroController : Controller
	{
		private readonly IMediator _mediator;

		public AdministrarCajeroController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Cajeros/Cajero
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> ListarCajeros()
		{
			var local = WebSession.Local;
			var response = await _mediator.Send(new ListarCajerosQuery { CodigoLocal = local });
			return Json(response);
		}

		[HttpPost]
		public async Task<ActionResult> ListarColaboradores(bool todos)
		{
			string local = "0";

			if (!todos)
				local = WebSession.Local;

			var empresa = WebSession.CodigoEmpresa;
			var response = await _mediator.Send(new ListarColaboradoresQuery { CodigoLocal = local, CodigoEmpresa = empresa });
			//return Json(response);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            return Content(json, "application/json");
        }

		[HttpPost]
		public async Task<JsonResult> AsignarCajero(List<CajeroDTO> cajeros)
		{
			cajeros.Select(x => { x.Usuario = WebSession.Login; return x; }).ToList();
			var respuesta = await _mediator.Send(new AsignarCajeroCommand { Cajeros = cajeros, TipoSO = WebSession.TipoSO, CodLocal = WebSession.Local });
			return Json(respuesta);

		}

		[HttpPost]
		public async Task<JsonResult> EliminarCajero(List<string> nroDocumentos)
		{
			var respuesta = await _mediator.Send(new EliminarCajeroCommand { NroDocumentos = nroDocumentos, Usuario = WebSession.Login, TipoSO = WebSession.TipoSO, CodLocal = WebSession.Local });
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> DescargarMaestro()
		{
			var respuesta = await _mediator.Send(new DescargarExcelCajerosCommand { CodigoLocal = WebSession.Local });
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> GenerarArchivo(List<string> cajeros)
		{
			var respuesta = await _mediator.Send(new GenerarArchivoCajeroCommand { TipoSO = WebSession.TipoSO, CodLocal = WebSession.Local, CodCajeros = cajeros });
			return Json(respuesta);
		}
	}
}