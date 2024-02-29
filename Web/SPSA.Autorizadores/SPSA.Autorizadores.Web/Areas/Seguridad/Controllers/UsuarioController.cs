using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Queries;
using SPSA.Autorizadores.Web.Utiles;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Seguridad.Controllers
{
	/// <summary>
	/// Controlador para la gestión de usuarios.
	/// </summary>
	public class UsuarioController : Controller
	{
		private readonly IMediator _mediator;

		/// <summary>
		/// Constructor para el controlador de usuarios.
		/// </summary>
		/// <param name="mediator">Un mediador para enviar o publicar solicitudes.</param>
		public UsuarioController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Acción para mostrar la vista de índice.
		/// </summary>
		/// <returns>La vista de índice.</returns>
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Acción para listar todos los usuarios.
		/// </summary>
		/// <returns>Una respuesta JSON con la lista de todos los usuarios.</returns>
		[HttpPost]
		public async Task<JsonResult> Listar()
		{
			var response = await _mediator.Send(new ListarUsuarioQuery());
			return Json(response);
		}

		/// <summary>
		/// Acción para crear un nuevo usuario.
		/// </summary>
		/// <param name="command">El comando para crear un nuevo usuario.</param>
		/// <returns>Una respuesta JSON con el resultado de la creación del usuario.</returns>
		[HttpPost]
		public async Task<JsonResult> CrearUsuario(CrearUsuarioCommand command)
		{
			command.UsuCreacion = WebSession.Login;
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para actualizar un usuario existente.
		/// </summary>
		/// <param name="command">El comando para actualizar un usuario existente.</param>
		/// <returns>Una respuesta JSON con el resultado de la actualización del usuario.</returns>
		[HttpPost]
		public async Task<JsonResult> ActualizarUsuario(ActualizarUsuarioCommand command)
		{
			command.UsuModifica = WebSession.Login;
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para listar todas las empresas.
		/// </summary>
		/// <returns>Una respuesta JSON con la lista de todas las empresas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarEmpresas(ListarEmpresasQuery query)
		{
			var respuesta = await _mediator.Send(query);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para asociar empresas a un usuario.
		/// </summary>
		/// <param name="command">El comando para asociar empresas a un usuario.</param>
		/// <returns>Una respuesta JSON con el resultado de la asociación de empresas.</returns>
		[HttpPost]
		public async Task<JsonResult> AsociarEmpresas(AsociarUsuarioEmpresaCommand command)
		{
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para listar todas las cadenas.
		/// </summary>
		/// <returns>Una respuesta JSON con la lista de todas las cadenas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarCadenas(ListarCadenasQuery query)
		{
			var respuesta = await _mediator.Send(query);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para asociar cadenas a un usuario.
		/// </summary>
		/// <param name="command">El comando para asociar cadenas a un usuario.</param>
		/// <returns>Una respuesta JSON con el resultado de la asociación de cadenas.</returns>
		[HttpPost]
		public async Task<JsonResult> AsociarCadenas(AsociarUsuarioCadenaCommand command)
		{
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para listar todas las cadenas.
		/// </summary>
		/// <returns>Una respuesta JSON con la lista de todas las regiones.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarRegiones(ListarRegionesQuery query)
		{
			var respuesta = await _mediator.Send(query);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para asociar regiones a un usuario.
		/// </summary>
		/// <param name="command">El comando para asociar regiones a un usuario.</param>
		/// <returns>Una respuesta JSON con el resultado de la asociación de regiones.</returns>
		[HttpPost]
		public async Task<JsonResult> AsociarRegiones(AsociarUsuarioRegionCommand command)
		{
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para listar todas las zonas.
		/// </summary>
		/// <returns>Una respuesta JSON con la lista de todas las zonas.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarZonas(ListarZonasQuery query)
		{
			var respuesta = await _mediator.Send(query);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para asociar zonas a un usuario.
		/// </summary>
		/// <param name="command">El comando para asociar zonas a un usuario.</param>
		/// <returns>Una respuesta JSON con el resultado de la asociación de zonas.</returns>
		[HttpPost]
		public async Task<JsonResult> AsociarZonas(AsociarUsuarioZonaCommand command)
		{
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para listar todas los locales.
		/// </summary>
		/// <returns>Una respuesta JSON con la lista de todas los locales.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarLocales(ListarLocalesQuery query)
		{
			var respuesta = await _mediator.Send(query);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para asociar locales a un usuario.
		/// </summary>
		/// <param name="command">El comando para asociar locales a un usuario.</param>
		/// <returns>Una respuesta JSON con el resultado de la asociación de locales.</returns>
		[HttpPost]
		public async Task<JsonResult> AsociarLocales(AsociarUsuarioLocalCommand command)
		{
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para listar todas los perfiles.
		/// </summary>
		/// <returns>Una respuesta JSON con la lista de todos los perfiles.</returns>
		[HttpPost]
		public async Task<JsonResult> ListarPerfiles(ListarPerfilesQuery query)
		{
			var respuesta = await _mediator.Send(query);
			return Json(respuesta);
		}

		/// <summary>
		/// Acción para asociar perfiles a un usuario.
		/// </summary>
		/// <param name="command">El comando para asociar perfiles a un usuario.</param>
		/// <returns>Una respuesta JSON con el resultado de la asociación de perfiles.</returns>
		[HttpPost]
		public async Task<JsonResult> AsociarPerfiles(AsociarUsuarioPerfilCommand command)
		{
			command.UsuCreacion = WebSession.Login;
			var respuesta = await _mediator.Send(command);
			return Json(respuesta);
		}

	}
}
