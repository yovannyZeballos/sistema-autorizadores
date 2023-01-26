using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Controllers
{
    public class AsignarLocalController : Controller
    {
        private readonly IMediator _mediator;

        public AsignarLocalController(IMediator mediator)
        {
            _mediator = mediator;

        }
        // GET: AsignarLocal
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarLocalesAsignar()
        {
            var respuesta = new ListarLocalesAsignarResponse();

            try
            {
                var colaboradoresCesados = await _mediator.Send(new ListarLocalesAsignarQuery());

                respuesta.Ok = true;
                respuesta.Locales = colaboradoresCesados.Locales;
                respuesta.Columnas = colaboradoresCesados.Columnas;
            }
            catch (System.Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Asignar(List<AsignarLocalRequest> request)
        {
            var respuesta = new RespuestaComunDTO();
            respuesta.Ok = true;
            try
            {
                foreach (var item in request)
                {
                    var rpta = await _mediator.Send(new AsignarLocalCommand
                    {
                        CodCadena = item.CodCadena,
                        CodLocal = item.CodLocal
                    });

                    if (!rpta.Ok)
                    {
                        respuesta.Ok = false;
                        respuesta.Mensaje += $" | {rpta.Mensaje}";
                    }
                        
                }
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