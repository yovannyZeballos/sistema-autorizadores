using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Newtonsoft.Json.Linq;
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
        public async Task<JsonResult> Listar(ListarValesRedimidosQuery request)
        {
            //try
            //{
            //    // Llamar al método que ejecuta el query
            //    var totalRecords = await _repositorioReportes.ObtenerTotalRegistrosAsync(codLocal, fechaInicio, fechaFin);
            //    var filteredRecords = totalRecords; // Ajustar si hay filtros adicionales
            //    var data = await _repositorioReportes.ListarValesRedimidosAsync(codLocal, fechaInicio, fechaFin, startRow, startRow + pageSize - 1);

            //    // Preparar la respuesta en el formato esperado por DataTables
            //    return Json(new
            //    {
            //        draw = draw,                 // Número de solicitud
            //        recordsTotal = totalRecords, // Total de registros sin filtros
            //        recordsFiltered = filteredRecords, // Total de registros después de aplicar filtros
            //        data = data                  // Datos de la página actual
            //    });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { draw = draw, error = ex.Message });
            //}

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
                return Json(new { draw = request.Draw, error = ex.Message });
            }

            //return Json(respuesta);
        }
    }
}