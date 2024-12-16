using System.Threading.Tasks;
using System;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Reportes.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Reportes.Commands;

namespace SPSA.Autorizadores.Web.Areas.Reportes.Controllers
{
    public class MaestroAutorizadoresController : Controller
    {
        private readonly IMediator _mediator;

        public MaestroAutorizadoresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Reportes/MaestroAutorizadores
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Descargar(DescargarMaeAutorizadoresCommand request)
        {
            try
            {
                var respuesta = await _mediator.Send(request);

                if (!respuesta.Ok)
                {
                    return Json(new { Mensaje = respuesta.Mensaje }, JsonRequestBehavior.AllowGet);
                }

                var archivoBytes = Convert.FromBase64String(respuesta.Archivo);

                return File(archivoBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", respuesta.NombreArchivo);
            }
            catch (Exception ex)
            {
                return Json(new { Mensaje = $"Error al descargar el archivo: {ex.Message}", Ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> ListarPaginado(ListarAutorizadoresPaginadoQuery request)
        {
            try
            {
                var respuesta = await _mediator.Send(request);

                return Json(new
                {
                    draw = request.Draw,
                    recordsTotal = respuesta.TotalRegistros,
                    recordsFiltered = respuesta.TotalRegistros,
                    respuesta.Data,
                    respuesta.Columnas,
                    respuesta.Ok
                });
            }
            catch (Exception ex)
            {
                return Json(new { draw = request.Draw, Mensaje = ex.Message, Ok = false });
            }
        }

    }
}