using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Autorizadores.Controllers
{
    public class PuestoController : Controller
    {
        private readonly IMediator _mediator;

        public PuestoController(IMediator mediator)
        {
            _mediator = mediator;
        }


        // GET: Puesto
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Listar(ListarPuestosQuery request)
        {
            var respuesta = new ListarPuestosResponse();

            try
            {
                var puestos = await _mediator.Send(request);

                respuesta.Ok = true;
                respuesta.Puestos = puestos.Puestos;
                respuesta.Columnas = puestos.Columnas;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> Actualizar(List<ActualizarPuestoCommand> puestos)
        {
            var respuesta = new StringBuilder();

            foreach (var item in puestos)
            {
                var respuestaComun = await _mediator.Send(item);
                if (!string.IsNullOrEmpty(respuestaComun.Mensaje))
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