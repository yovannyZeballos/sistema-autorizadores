using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Web.Extensiones;
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
using System.Web.UI.WebControls;

namespace SPSA.Autorizadores.Web.Controllers
{
    public class AutorizadorController : Controller
    {
        private readonly IMediator _mediator;

        public AutorizadorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Autorizador
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarColaborador()
        {
            var respuesta = new ListarColaboradorResponse();
            var local = WebSession.LocalOfiplan;

            try
            {
                var colaboradores = await _mediator.Send(new ListarColaboradoresQuery { CodigoLocal = local });
                respuesta.Ok = true;
                respuesta.Colaboradores = colaboradores;
            }
            catch (System.Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarTodosColaborador()
        {
            var respuesta = new ListarColaboradorResponse();

            try
            {
                var colaboradores = await _mediator.Send(new ListarColaboradoresQuery { CodigoLocal = "0" });
                respuesta.Ok = true;
                respuesta.Colaboradores = colaboradores;
            }
            catch (System.Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarAutorizador()
        {
            var respuesta = new ListarAutorizadorResponse();
            var local = WebSession.Local;

            try
            {
                var autorizadoresDatatable = await _mediator.Send(new ListarAutorizadoresQuery { CodigoLocal = local });
                respuesta.Columnas = new List<string>();
                foreach (DataColumn colum in autorizadoresDatatable.Columns)
                {
                    respuesta.Columnas.Add(colum.ColumnName);
                }

                var lst = autorizadoresDatatable.AsEnumerable()
                         .Select(r => r.Table.Columns.Cast<DataColumn>()
                         .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                      ).ToDictionary(z => z.Key.Replace(" ","").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
                   ).ToList();


                respuesta.Ok = true;
                respuesta.Autorizadores = lst;
            }
            catch (System.Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> AsignarAutorizador(List<CrearAutorizadorCommand> autorizadores)
        {
            var respuesta = new StringBuilder();
            var codigoLocal = WebSession.Local;
            var tipoSO = WebSession.TipoSO;
            var usuario = WebSession.Login;

            foreach (var autorizador in autorizadores)
            {
                autorizador.CodLocal = Convert.ToInt32(codigoLocal);
                autorizador.UsuarioCreacion = usuario;
                var respuestaComun = await _mediator.Send(autorizador);
                respuesta.AppendLine(respuestaComun.Mensaje);
            }

            var respuestaGeneracion = await _mediator.Send(new GenerarArchivoCommand { TipoSO = tipoSO });
            if (!respuestaGeneracion.Ok)
            {
                respuesta.AppendLine("No se pudieron crear los archivos:");
                respuesta.AppendLine(respuestaGeneracion.Mensaje);
            }
            else
            {
                respuesta.AppendLine("Archivo creado:");

                foreach (var item in respuestaGeneracion.Mensaje.Split('\n'))
                {
                    if (item != "null" && item != "")
                        respuesta.AppendLine(item.Split('|').Count() == 0 ? "" : $"{item.Split('|')[0]}/{item.Split('|')[1]}");

                }
            }


            return Json(new RespuestaComunDTO
            {
                Ok = true,
                Mensaje = respuesta.ToString()
            });
        }

        [HttpPost]
        public async Task<JsonResult> ActualizarEstadoArchivoAutorizador(List<ActualizarEstadoArchivoCommand> autorizadores)
        {
            var respuesta = new StringBuilder();
            var codigoLocal = WebSession.Local;
            var tipoSO = WebSession.TipoSO;
            var usuario = WebSession.Login;

            foreach (var autorizador in autorizadores)
            {
                autorizador.CodLocal = Convert.ToInt32(codigoLocal);
                await _mediator.Send(autorizador);
            }

            var respuestaGeneracion = await _mediator.Send(new GenerarArchivoCommand { TipoSO = tipoSO });
            if (!respuestaGeneracion.Ok)
            {
                respuesta.AppendLine("No se pudieron crear los archivos:");
                respuesta.AppendLine(respuestaGeneracion.Mensaje);
            }
            else
            {
                respuesta.AppendLine("Archivos creados:");

                foreach (var item in respuestaGeneracion.Mensaje.Split('\n'))
                {
                    if (item != "null" && item != "")
                        respuesta.AppendLine(item.Split('|').Count() == 0 ? "" : $"{item.Split('|')[0]}/{item.Split('|')[1]}");

                }
            }


            return Json(new RespuestaComunDTO
            {
                Ok = true,
                Mensaje = respuesta.ToString()
            });
        }

        [HttpPost]
        public async Task<JsonResult> EliminarAutorizador(List<EliminarAutorizadorCommand> autorizadores)
        {
            var respuesta = new StringBuilder();
            var usuario = WebSession.Login;

            foreach (var autorizador in autorizadores)
            {
                autorizador.UsuarioCreacion = usuario;
                var respuestaComun = await _mediator.Send(autorizador);
                respuesta.AppendLine(respuestaComun.Mensaje);
            }

            return Json(new RespuestaComunDTO
            {
                Ok = true,
                Mensaje = respuesta.ToString()
            });
        }
    }
}