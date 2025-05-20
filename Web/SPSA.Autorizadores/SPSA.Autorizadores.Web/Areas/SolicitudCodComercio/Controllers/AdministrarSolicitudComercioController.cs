using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Queries;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.SolicitudCodComercio.Controllers
{
    public class AdministrarSolicitudComercioController : Controller
    {
        private readonly IMediator _mediator;

        public AdministrarSolicitudComercioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: SolicitudCodComercio/AdministrarSolicitudComercio
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CrearEditarMaeCodComercio(CrearEditarMaeCodComercioCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        //[HttpPost]
        //public async Task<JsonResult> Obtener(ObtenerMaeColaboradorIntQuery request)
        //{
        //    var respuesta = await _mediator.Send(request);
        //    return Json(respuesta);
        //}

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarSolicitudCComercioCabQuery request)
        {

            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarSolicitudes()
        {
            var respuesta = new RespuestaComunExcelDTO();
            foreach (var fileKey in Request.Files)
            {
                HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
                if (archivo is null)
                {
                    respuesta = new RespuestaComunExcelDTO
                    {
                        Errores = new List<ErroresExcelDTO>(),
                        Ok = false,
                        Mensaje = "Se encontraron algunos errores en el archivo"
                    };
                    respuesta.Errores.Add(new ErroresExcelDTO
                    {
                        Fila = 1,
                        Mensaje = "No se ha seleccionado ningun archivo."
                    });
                    return Json(respuesta);
                }
                else
                {
                    respuesta = await _mediator.Send(new ImportarSolicitudCodComercioCommand
                    {
                        Archivo = archivo,
                        Usuario = WebSession.Login
                    });
                }
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarComercios()
        {
            var respuesta = new RespuestaComunExcelDTO();

            foreach (var fileKey in Request.Files)
            {
                HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];

                var nroSolicitud = Request.Form["nroSolicitud"];
                var localesJson = Request.Form["localesJson"];
                var locales = new List<SolicitudCComercioDetDTO>();
                if (!string.IsNullOrWhiteSpace(localesJson))
                {
                    locales = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SolicitudCComercioDetDTO>>(localesJson);
                }

                respuesta = await _mediator.Send(new ImportarMaeLocalComercioCommand
                {
                    Archivo = archivo,
                    Usuario = WebSession.Login,
                    NroSolicitud = Convert.ToDecimal(nroSolicitud),
                    Locales = locales
                });
            }

            return Json(respuesta);

            //var respuesta = new RespuestaComunExcelDTO();
            //foreach (var fileKey in Request.Files)
            //{
            //    HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
            //    if (archivo is null)
            //    {
            //        respuesta = new RespuestaComunExcelDTO
            //        {
            //            Errores = new List<ErroresExcelDTO>(),
            //            Ok = false,
            //            Mensaje = "Se encontraron algunos errores en el archivo"
            //        };
            //        respuesta.Errores.Add(new ErroresExcelDTO
            //        {
            //            Fila = 1,
            //            Mensaje = "No se ha seleccionado ningun archivo."
            //        });
            //        return Json(respuesta);
            //    }
            //    else
            //    {
            //        respuesta = await _mediator.Send(new ImportarMaeLocalComercioCommand
            //        {
            //            Archivo = archivo,
            //            Usuario = WebSession.Login
            //        });
            //    }
            //}

            //return Json(respuesta);
        }
    }
}