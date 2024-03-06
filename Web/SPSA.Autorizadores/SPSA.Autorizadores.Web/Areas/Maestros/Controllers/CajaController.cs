using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using System.Threading.Tasks;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using SPSA.Autorizadores.Aplicacion.Features.Caja.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Caja.Command;


namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class CajaController : Controller
    {
        private readonly IMediator _mediator;

        public CajaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerCaja(ObtenerMaeCajaQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarCaja(ListarMaeCajaQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarCaja(ActualizarMaeCajaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarExcelCaja(HttpPostedFileBase archivoExcel)
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
                var command = new ImportarMaeCajaCommand { ArchivoExcel = archivoExcel.InputStream };
                var response = await _mediator.Send(command);

                return Json(response);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CrearCaja(CrearMaeCajaCommand command)
        {
            //command.CodLocalOfiplan = (command.CodLocalOfiplan is null) ? "" : command.CodLocalOfiplan;
            //command.NomLocalOfiplan = (command.NomLocalOfiplan is null) ? "" : command.NomLocalOfiplan;

            //command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarCajas(EliminarMaeCajasCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarCaja(DescargarMaeCajaCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

    }
}