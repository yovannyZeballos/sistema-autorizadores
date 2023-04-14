using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using SPSA.Autorizadores.Web.Utiles;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Autorizadores.Controllers
{
    public class EliminarAutorizadorController : Controller
    {
        private readonly IMediator _mediator;

        public EliminarAutorizadorController(IMediator mediator)
        {
            _mediator = mediator;

        }
        // GET: ColaboradoresCesados
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ListarColaboradoresCesados()
        {
            var respuesta = new ListarColaboradoresCesadosResponse();

            try
            {
                var colaboradoresCesados = await _mediator.Send(new ListarColaboradoresCesadosQuery());

                respuesta.Ok = true;
                respuesta.Colaboradores = colaboradoresCesados.Colaboradores;
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
        public async Task<JsonResult> EliminarAutorizador(List<EliminarAutorizadorCommand> autorizadores)
        {
            var respuesta = new RespuestaComunDTO();
            respuesta.Ok = true;

            var usuario = WebSession.Login;

            foreach (var autorizador in autorizadores)
            {
                autorizador.UsuarioCreacion = usuario;
                var rpta = await _mediator.Send(autorizador);

                if (!rpta.Ok)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje += $" | {rpta.Mensaje}";
                }

            }

            return Json(respuesta);
        }

    }
}