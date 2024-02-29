using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Seguridad.Controllers
{
	/// <summary>
	/// Controlador para la gestión de perfiles de seguridad.
	/// </summary>
	public class PerfilController : Controller
	{
		private readonly IMediator _mediator;

		/// <summary>
		/// Constructor de la clase PerfilController.
		/// </summary>
		/// <param name="mediator">Instancia de IMediator para la comunicación con los manejadores de comandos y consultas.</param>
		public PerfilController(IMediator mediator)
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
		/// Acción para listar los perfiles de seguridad.
		/// </summary>
		/// <returns>JsonResult con la lista de perfiles de seguridad.</returns>
		public async Task<JsonResult> Listar()
		{
			var response = await _mediator.Send(new ListarPerfilesQuery());
			return Json(response);
		}

		/// <summary>
		/// Acción para crear un nuevo perfil de seguridad.
		/// </summary>
		/// <param name="command">Comando para crear el sistema de seguridad.</param>
		/// <returns>JsonResult con la respuesta de la creación del sistema de seguridad.</returns>
		[HttpPost]
		public async Task<JsonResult> CrearPerfil(CrearPerfilCommand command)
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
		public async Task<JsonResult> ActualizarPerfil(ActualizarPerfilCommand command)
		{
			command.UsuModifica = WebSession.Login;
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> AsociarMenus(AsociarMenusCommand command)
		{
			command.UsuCreacion = WebSession.Login;
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		[HttpPost]
		public async Task<JsonResult> ListarMenus(ListarMenusQuery query)
		{
			var respuesta = await _mediator.Send(query);
			return Json(respuesta);
		}

	}
}