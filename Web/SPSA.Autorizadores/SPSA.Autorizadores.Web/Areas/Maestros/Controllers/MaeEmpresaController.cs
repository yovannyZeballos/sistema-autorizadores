using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cadenas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaeEmpresaController : Controller
    {
        private readonly IMediator _mediator;

        public MaeEmpresaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Maestros/Empresa
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarEmpresasAsociadas(ListarEmpresasAsociadasQuery request)
        {
            var respose = await _mediator.Send(request);
            return Json(respose);
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerEmpresa(ObtenerMaeEmpresaQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ListarEmpresa(ListarMaeEmpresaQuery request)
        {
            var response = await _mediator.Send(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> CrearEmpresa(CrearMaeEmpresaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarEmpresa(ActualizarMaeEmpresaCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarExcelEmpresa(HttpPostedFileBase archivoExcel)
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
                var command = new ImportarMaeEmpresaCommand { ArchivoExcel = archivoExcel.InputStream };
                var response = await _mediator.Send(command);

                return Json(response);
            }
        }
    }
}