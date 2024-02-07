using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Seguridad.Controllers
{
	/// <summary>
	/// Controlador para la gestión de sistemas de seguridad.
	/// </summary>
	public class SistemaController : Controller
	{
		private readonly IMediator _mediator;

		/// <summary>
		/// Constructor de la clase SistemaController.
		/// </summary>
		/// <param name="mediator">Instancia de IMediator para la comunicación con los manejadores de comandos y consultas.</param>
		public SistemaController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Acción para mostrar la vista principal del controlador.
		/// </summary>
		/// <returns>Vista principal del controlador.</returns>
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		/// <summary>
		/// Acción para listar los sistemas de seguridad.
		/// </summary>
		/// <returns>JsonResult con la lista de sistemas de seguridad.</returns>
		public async Task<JsonResult> Listar()
		{
			var response = await _mediator.Send(new ListarSistemasQuery());
			return Json(response);
		}

		/// <summary>
		/// Acción para crear un nuevo sistema de seguridad.
		/// </summary>
		/// <param name="command">Comando para crear el sistema de seguridad.</param>
		/// <returns>JsonResult con la respuesta de la creación del sistema de seguridad.</returns>
		[HttpPost]
		public async Task<JsonResult> CrearSistema(CrearSistemaCommand command)
		{
			command.UsuCreacion = WebSession.Login;
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para actualizar un sistema de seguridad existente.
		/// </summary>
		/// <param name="command">Comando para actualizar el sistema de seguridad.</param>
		/// <returns>JsonResult con la respuesta de la actualización del sistema de seguridad.</returns>
		[HttpPost]
		public async Task<JsonResult> ActualizarSistema(ActualizarSistemaCommand command)
		{
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para obtener un sistema de seguridad.
		/// </summary>
		/// <param name="request">Consulta para obtener el sistema de seguridad.</param>
		/// <returns>JsonResult con la respuesta de la obtención del sistema de seguridad.</returns>
		[HttpPost]
		public async Task<JsonResult> ObtenerSistema(ObtenerSistemaQuery request)
		{
			var response = await _mediator.Send(request);
			return Json(response);
		}


	}
}