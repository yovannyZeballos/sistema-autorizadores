﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.Queries;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaeHorarioController : Controller
    {
        private readonly IMediator _mediator;

        public MaeHorarioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Maestros/MaeHorario
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Obtener(ObtenerMaeHorarioQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Listar(ListarMaeHorarioQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Crear(CrearMaeHorarioCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            command.FecCreacion = DateTime.Today;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Actualizar(ActualizarMaeHorarioCommand command)
        {
            command.UsuModifica = WebSession.Login;
            command.FecModifica = DateTime.Today;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Eliminar(EliminarMaeHorarioCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarPorLocal(DescargarMaeHorarioCommand request)
        {
            var respuesta = await _mediator.Send(request);
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
                    var command = new ImportarMaeHorarioCommand { ArchivoExcel = archivo.InputStream };
                    var response = await _mediator.Send(command);

                    return Json(response);
                }
            }

            return Json(respuesta);
        }

        [HttpPost]
        public ActionResult FormularioCrear(MaeHorarioDTO model)
        {
            return PartialView("_CrearMaeHorario", model);
        }

        [HttpPost]
        public ActionResult FormularioEditar(MaeHorarioDTO model)
        {
            return PartialView("_EditarMaeHorario", model);
        }

    }
}