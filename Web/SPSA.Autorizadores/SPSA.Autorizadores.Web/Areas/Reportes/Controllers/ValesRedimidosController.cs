using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Reportes.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Reportes.Queries;

namespace SPSA.Autorizadores.Web.Areas.Reportes.Controllers
{
    public class ValesRedimidosController : Controller
    {
        private readonly IMediator _mediator;

        public ValesRedimidosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Reportes/ValesRedimidos
        public ActionResult Index()
        {
            ViewBag.PowerBiEmbedUrl = "https://app.powerbi.com/view?r=eyJrIjoiZjMwY2Q0YjAtYjMwZC00OWJhLWE0MTctZDk1ODU4YTA4NGQ0IiwidCI6IjVkYWFiZTY1LWZmNTMtNDMzNi1hNDQ4LTJmNjFlY2YwYjA1OSIsImMiOjR9";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Descargar(DescargarValesRedimidosCommand request)
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
        public async Task<JsonResult> ListarPaginado(ListarValesRedimidosQuery request)
        {
            try
            {
                var respuesta = await _mediator.Send(request);

                return Json(new
                {
                    draw = request.Draw,
                    recordsTotal = respuesta.TotalRegistros, // Total de registros sin paginar
                    recordsFiltered = respuesta.TotalRegistros, // Total de registros después de filtros (puede variar si hay filtros adicionales)
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