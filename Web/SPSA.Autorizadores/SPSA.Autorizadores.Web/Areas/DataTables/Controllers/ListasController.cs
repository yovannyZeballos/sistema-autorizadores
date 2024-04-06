using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.DataTableSGP.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.DataTables.Controllers
{
    public class ListasController : Controller
    {
        private readonly IMediator _mediator;

        public ListasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Autorizador
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarUsuario()
        {
            var respuesta = new ListarUsuarioResponse();
            try
            {
                var usuariosDatatable = await _mediator.Send(new DtUsuariosQuery { });
                respuesta.Columnas = new List<string>();
                foreach (DataColumn colum in usuariosDatatable.Columns)
                {
                    respuesta.Columnas.Add(colum.ColumnName);
                }

                var lst = usuariosDatatable.AsEnumerable()
                        .Select(r => r.Table.Columns.Cast<DataColumn>()
                        .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                     ).ToDictionary(z => z.Key.Replace(" ", "")
                                              .Replace(".", "")
                                              .Replace("á", "a")
                                              .Replace("é", "e")
                                              .Replace("í", "i")
                                              .Replace("ó", "o")
                                              .Replace("ú", "u"), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
                  ).ToList();


                respuesta.Ok = true;
                respuesta.Usuarios = lst;
            }
            catch (System.Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        
    }
}
