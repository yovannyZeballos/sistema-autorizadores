using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Queries;
using SPSA.Autorizadores.Web.Utiles;
using Stimulsoft.Report.Events;

namespace SPSA.Autorizadores.Web.Areas.SolicitudASR.Controllers
{
    public class SolicitudUsuarioController : Controller
    {
        private readonly IMediator _mediator;

        public SolicitudUsuarioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: SolicitudASR/SolicitudUsuario
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarSolicitudUsuarioQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CrearSolicitud(CrearSolicitudUsuarioCommand command)
        {
            command.UsuSolicita = WebSession.Login;
            var respuesta = await _mediator.Send(command);

            if (respuesta.Ok)
            {
                //var emailCommand = new EnviarCorreoCommand
                //{
                //    CodProceso = 37,
                //    CodEmpresa = "01",
                //    Subject = "Solicitud Creada",
                //    Message = $"La solicitud de usuario {command.CodColaborador} se ha creado exitosamente."
                //};

                //var emailResponse = await _mediator.Send(emailCommand);
                //if (!emailResponse.Ok)
                //{
                //    respuesta.Mensaje = $"La solicitud se creó, pero falló el envío del correo: {emailResponse.Mensaje}";
                //}
            }

            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarSolicitud(EliminarSolicitudUsuarioCommand command)
        {
            command.UsuElimina = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> DescargarSolicitudes(DescargarSolicitudesUsuarioCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }
    }
}