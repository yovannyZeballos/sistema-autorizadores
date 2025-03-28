﻿using MediatR;
using System.Globalization;
using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using SPSA.Autorizadores.Aplicacion.DTO;
using System.Collections.Generic;
using System.Web;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaeLocalController : Controller
    {
        private readonly IMediator _mediator;

        public MaeLocalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index()
        {
            object fechaActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return View(fechaActual);
            //return View();
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerLocal(ObtenerMaeLocalQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarLocal(ListarMaeLocalQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarLocalPorEmpresa(ListarMaeLocalPorEmpresaQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarLocalPorEmpresaAsociada(ListarEmpresasAsociadasQuery request)
        {
            var respose = await _mediator.Send(request);
            return Json(respose);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarLocal(ActualizarMaeLocalCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarExcel()
        {
            var respuesta = new RespuestaComunExcelDTO();
            foreach (var fileKey in Request.Files)
            {
                HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
                if (archivo is null)
                {
                    var response = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
                    response.Ok = false;
                    response.Mensaje = "Se encontraron algunos errores en el archivo";
                    response.Errores.Add(new ErroresExcelDTO
                    {
                        Fila = 1,
                        Mensaje = "No se ha seleccionado ningun archivo."
                    });

                    return Json(response);
                }
                else
                {
                    var command = new ImportarMaeLocalCommand { ArchivoExcel = archivo.InputStream };
                    var response = await _mediator.Send(command);

                    return Json(response);
                }
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> CrearLocal(CrearMaeLocalCommand command)
        {
            command.CodLocalOfiplan = (command.CodLocalOfiplan is null) ? "" : command.CodLocalOfiplan;
            command.NomLocalOfiplan = (command.NomLocalOfiplan is null) ? "" : command.NomLocalOfiplan;

            //command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarLocal(DescargarMaeLocalCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarLocalPorEmpresa(DescargarMaeLocalPorEmpresaCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}