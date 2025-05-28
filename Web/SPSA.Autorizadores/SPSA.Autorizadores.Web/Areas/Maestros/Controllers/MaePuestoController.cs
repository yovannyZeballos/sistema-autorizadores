using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.DTOs;
using SPSA.Autorizadores.Aplicacion.ViewModel;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaePuestoController : Controller
    {
        private readonly IMediator _mediator;

        public MaePuestoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Maestros/MaePuesto
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarMaePuestoQuery request)
        {

            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ModificarPuesto(ActualizarMaePuestoCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Importar()
        {
            var respuesta = new RespuestaComunExcelDTO();
            foreach (var fileKey in Request.Files)
            {
                HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
                if (archivo is null)
                {
                    respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Se encontraron algunos errores en el archivo";
                    respuesta.Errores.Add(new ErroresExcelDTO
                    {
                        Fila = 1,
                        Mensaje = "No se ha seleccionado ningun archivo."
                    });
                    return Json(respuesta);
                }
                else
                {
                    respuesta = await _mediator.Send(new ImportarMaeColaboradorExtCommand
                    {
                        Archivo = archivo,
                        Usuario = WebSession.Login,
                        JerarquiaOrganizacional = WebSession.JerarquiaOrganizacional
                    });
                }
            }

            return Json(respuesta);
        }

    }
}