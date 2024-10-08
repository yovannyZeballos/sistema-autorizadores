﻿using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
	public class CierreEODController : Controller
	{
		private readonly IMediator _mediator;

		public CierreEODController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: MonitorCierre
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> ListarMonitor(ListarLocalMonitorQuery request)
		{
			request.Tipo = (int)TipoMonitor.CIERRE_FIN_DIA;
			var response = await _mediator.Send(request);
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> Procesar(ProcesarMonitorCommand request)
		{
			var response = await _mediator.Send(request);
			return Json(response);

		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> ListarEmpresas()
		{
			var respuesta = new ListarEmpresaResponse();

			try
			{
				var empresas = await _mediator.Send(new ListarEmpresasMonitorQuery());
				respuesta.Ok = true;
				respuesta.Empresas = empresas;
			}
			catch (System.Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return Json(respuesta, JsonRequestBehavior.AllowGet);
		}
	}
}