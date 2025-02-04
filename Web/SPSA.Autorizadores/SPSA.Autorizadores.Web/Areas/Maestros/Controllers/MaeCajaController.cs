using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.DTOs;
using System.Linq;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaeCajaController : Controller
    {
        private readonly IMediator _mediator;

        public MaeCajaController(IMediator mediator)
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
        public async Task<JsonResult> ListarCajasActivas(ListarMaeCajaQuery request)
        {
            var respuesta = await _mediator.Send(request);

            var localesActivos = respuesta.Data.Where(x => x.TipEstado == "A").ToList();
            respuesta.Data = localesActivos;

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarCaja(ActualizarMaeCajaCommand command)
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
                    var command = new ImportarMaeCajaCommand { ArchivoExcel = archivo.InputStream };
                    var response = await _mediator.Send(command);

                    return Json(response);
                }
            }

            return Json(respuesta);
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
        public async Task<JsonResult> DescargarCajaPorLocal(DescargarMaeCajaCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarCajaPorEmpresa(DescargarMaeCajaPorEmpresaCommand request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta);
        }

        [HttpPost]
        public ActionResult CrearEditarCaja(MaeCajaDTO model)
        {
            return PartialView("_CrearEditarCaja", model);
        }

    }
}