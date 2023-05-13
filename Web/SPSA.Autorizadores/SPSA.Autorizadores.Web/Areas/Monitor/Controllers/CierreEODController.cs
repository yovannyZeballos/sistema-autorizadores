using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Monitor.Controllers
{
    public class CierreEODController : Controller
    {
        private readonly IMediator _mediator;

        public CierreEODController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: MonitorCierre
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarMonitor(ListarMonitorRequest request)
        {
            var respuesta = new ListarMonitorResponse();
            var local = WebSession.LocalOfiplan;
            try
            {
                var fechaValida = DateTime.TryParseExact(request.Fecha, "dd/MM/yyyy", new CultureInfo("es-PE"), DateTimeStyles.None, out DateTime fecha);

                if (!fechaValida)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "El formato de la fecha ingresada es invalida";
                    return Json(respuesta);
                }

                var localesDatatable = await _mediator.Send(new ListarLocalMonitorQuery { CodEmpresa = request.CodEmpresa, Estado = request.Estado, Fecha = fecha });
                respuesta.Columnas = new List<string>();
                foreach (DataColumn colum in localesDatatable.Columns)
                {
                    respuesta.Columnas.Add(colum.ColumnName);
                }

                var lst = localesDatatable.AsEnumerable()
                        .Select(r => r.Table.Columns.Cast<DataColumn>()
                        .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                     ).ToDictionary(z => z.Key.Replace(" ", "")
                                              .Replace(".", "")
                                              .Replace("á", "a")
                                              .Replace("é", "e")
                                              .Replace("í", "i")
                                              .Replace("ó", "o")
                                              .Replace("ú", "u"), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy HH:mm:ss") : z.Value)
                  ).ToList();

                respuesta.Locales = lst;
                respuesta.Ok = true;

                if (lst.Count == 0)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encuentra información de cierre sobre la fecha ingresada.";
                    return Json(respuesta);
                }

                
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Procesar(ProcesarRequest request)
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                var fechaValida = DateTime.TryParseExact(request.Fecha, "dd/MM/yyyy", new CultureInfo("es-PE"), DateTimeStyles.None, out DateTime fecha);
                if (!fechaValida)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "El formato de la fecha ingresada es invalida";
                    return Json(respuesta);
                }

                respuesta = await _mediator.Send(new ProcesarMonitorCommand { CodEmpresa = request.CodEmpresa, FechaCierre = fecha });
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> ListarEmpresas()
        {
            var respuesta = new ListarEmpresaResponse();

            try
            {
                var empresas = await _mediator.Send(new ListarEmpresasMonitorQuery());
                respuesta.Ok = true;
                respuesta.Empresas = empresas;
            }
            catch (System.Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}