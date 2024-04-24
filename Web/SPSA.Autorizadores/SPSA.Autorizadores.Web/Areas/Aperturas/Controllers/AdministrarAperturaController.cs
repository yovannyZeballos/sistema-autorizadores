using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Aperturas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Aperturas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Aperturas.Controllers
{
    public class AdministrarAperturaController : Controller
    {
        private readonly IMediator _mediator;

        public AdministrarAperturaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Aperturas/Apertura
        public ActionResult Index()
        {
            AperturaDTO apertura = new AperturaDTO();
            return View(apertura);
        }

        [HttpPost]
        public ActionResult CrearEditarApertura(AperturaDTO model)
        {
            return PartialView("_CrearEditarApertura", model);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerApertura(ObtenerAperturaQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ListarApertura(ListarAperturaQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> CrearApertura(CrearAperturaCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            command.FecCreacion = DateTime.Now;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarApertura(ActualizarAperturaCommand command)
        {
            command.UsuModifica = WebSession.Login;
            command.FecModifica = DateTime.Now;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarExcelApertura(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel is null)
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
                var command = new ImportarAperturaCommand { ArchivoExcel = archivoExcel.InputStream };
                var response = await _mediator.Send(command);

                return Json(response);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DescargarExcelApertura(DescargarAperturaCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }
    }
}