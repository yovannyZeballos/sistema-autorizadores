using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Autorizadores.Controllers
{
    public class AutorizadoresMassController : Controller
    {

        private readonly IMediator _mediator;

        public AutorizadoresMassController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: AutorizadoresMass
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<JsonResult> ListarColaboradores()
        //{
        //    var respuesta = new ListarColaboradoresMassResponse();
        //    var local = WebSession.Local;

        //    try
        //    {
        //        var autorizadoresDatatable = await _mediator.Send(new ListarColaboradoresMassQuery { CodigoEmpresa = WebSession.CodigoEmpresa });
        //        respuesta.Columnas = new List<string>();
        //        foreach (DataColumn colum in autorizadoresDatatable.Columns)
        //        {
        //            respuesta.Columnas.Add(colum.ColumnName);
        //        }

        //        var lst = autorizadoresDatatable.AsEnumerable()
        //                 .Select(r => r.Table.Columns.Cast<DataColumn>()
        //                 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
        //              ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
        //           ).ToList();


        //        respuesta.Ok = true;
        //        respuesta.Colaboradores = lst;
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Ok = false;
        //        respuesta.Mensaje = ex.Message;
        //    }

        //    return Json(respuesta);
        //}

        //[HttpPost]
        //public async Task<JsonResult> AsignarAutorizador(List<CrearAutorizadorCommand> autorizadores)
        //{
        //    var respuesta = new StringBuilder();
        //    var codigoLocal = WebSession.Local;
        //    var tipoSO = WebSession.TipoSO;
        //    var usuario = WebSession.Login;

        //    foreach (var autorizador in autorizadores)
        //    {
        //        autorizador.UsuarioCreacion = usuario;
        //        var respuestaComun = await _mediator.Send(autorizador);
        //        //respuesta.AppendLine(respuestaComun.Mensaje);
        //    }

        //    respuesta.AppendLine("Autorizadores creados");

        //    var respuestaGeneracion = await _mediator.Send(new GenerarArchivoCommand { TipoSO = tipoSO });
        //    if (!respuestaGeneracion.Ok)
        //    {
        //        respuesta.AppendLine("No se pudieron crear los archivos:");
        //        respuesta.AppendLine(respuestaGeneracion.Mensaje);
        //    }
        //    else
        //    {
        //        respuesta.AppendLine("Archivo creado:");

        //        foreach (var item in respuestaGeneracion.Mensaje.Split('\n'))
        //        {
        //            if (item != "null" && item != "")
        //                respuesta.AppendLine(item.Split('|').Count() == 0 ? "" : $"{item.Split('|')[0]}/{item.Split('|')[1]}");

        //        }
        //    }


        //    return Json(new RespuestaComunDTO
        //    {
        //        Ok = true,
        //        Mensaje = respuesta.ToString()
        //    });
        //}
    }
}