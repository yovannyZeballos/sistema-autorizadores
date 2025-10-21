using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Login.Queries;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Web.Models.Intercambio;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace SPSA.Autorizadores.Web.Controllers
{
	public class LoginController : Controller
	{
		private readonly IMediator _mediator;
		private readonly ILogger _log = SerilogClass._log;
		public LoginController(IMediator mediator)
		{
			_mediator = mediator;
		}



		// GET: Login
		public ActionResult Index()
		{
			WebSession.SistemaVersion = ConfigurationManager.AppSettings["SistemaVersion"].ToString();
			WebSession.SistemaAmbiente = ConfigurationManager.AppSettings["SistemaAmbiente"].ToString();
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<JsonResult> Index(LoginCommand command)
		{
			var respuesta = new RespuestaComunDTO();
			try
			{
				var usuario = await _mediator.Send(command);
				respuesta.Ok = usuario.Ok;

				if (usuario.Ok)
				{
					_log.Information($"Usuario: {command.Usuario}, Inicio de sesion exitoso");
					WebSession.Login = command.Usuario;
					WebSession.UserName = usuario.NombreUsuario;
					WebSession.Permisos = usuario.Aplicacion.Permisos;
					WebSession.Locales = usuario.Locales;
					WebSession.SistemaVersion = ConfigurationManager.AppSettings["SistemaVersion"].ToString();
					WebSession.SistemaAmbiente = ConfigurationManager.AppSettings["SistemaAmbiente"].ToString();
					WebSession.MenusAsociados = usuario.MenusAsociados.OrderBy(x => x.CodMenu).ToList();




				}
				else
				{
					respuesta.Mensaje = usuario.Mensaje;
					respuesta.Ok = usuario.Ok;
				}
			}
			catch (System.Exception ex)
			{
				_log.Error(ex, ex.Message);
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return Json(respuesta);

		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> ListarEmpresas()
		{
			var response = await _mediator.Send(new ListarEmpresasQuery());
			return Json(response, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<JsonResult> ListarLocales(string ruc)
		{
			var respuesta = new ListarLocalResponse();

			try
			{
				WebSession.Ruc = ruc;
				var login = WebSession.Login;
				//var localesUsuario = WebSession.Locales;
				//var locales = await _mediator.Send(new ListarLocalesQuery { Ruc = ruc, Locales = localesUsuario });
				var locales = await _mediator.Send(new ListarLocalesQuery { Ruc = ruc, Locales = WebSession.Locales });
				//WebSession.LocalesAsignadosXEmpresa = locales;
				respuesta.Ok = true; ;
				respuesta.Locales = locales;
			}
			catch (System.Exception ex)
			{
				_log.Error(ex, ex.Message);
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return Json(respuesta, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public async Task<JsonResult> GuardarLocalSession(ObtenerJerarquiaOrganizacionalQuery query)
		{
			var response = new RespuestaComunDTO();
			try
			{
				query.Usuario = WebSession.Login;
				var jerarquia = await _mediator.Send(query);

				if (!jerarquia.Ok)
				{
					response.Ok = false;
					response.Mensaje = jerarquia.Mensaje;
					return Json(response);
				}

				WebSession.JerarquiaOrganizacional = jerarquia;

                var local = await _mediator.Send(new ObtenerLocalQuery { Codigo = query.CodLocal });

                if (local != null)
                {
                    WebSession.Local = query.CodLocal;
                    WebSession.TipoSO = local.TipoSO;
                    WebSession.LocalOfiplan = local.CodigoOfiplan;
                    WebSession.NombreLocal = $"{local.Nombre} ({(local.Manual == "S" ? "MANUAL" : "CON TARJETA")})";
                    WebSession.CodigoEmpresa = local.CodigoEmpresa;
                }
                else
                {
                    var maeLocal = await _mediator.Send(new ObtenerMaeLocalQuery
                    {
                        CodEmpresa = query.CodEmpresa,
                        CodCadena = query.CodCadena,
                        CodRegion = query.CodRegion,
                        CodZona = query.CodZona,
                        CodLocal = query.CodLocal
                    });

                    WebSession.Local = query.CodLocal;
                    WebSession.NombreLocal = maeLocal.Data?.NomLocal ?? "LOCAL DESCONOCIDO";
                    WebSession.CodigoEmpresa = query.CodEmpresa;
                }


                response.Ok = true;
			}
			catch (System.Exception ex)
			{
				_log.Error(ex, ex.Message);
				response.Ok = false;
				response.Mensaje = ex.Message;
			}

			return Json(response);

		}

		[HttpGet]
		public RedirectToRouteResult Logout()
		{
			FormsAuthentication.SignOut();
			HttpContext.Response.Cookies.Clear();
			HttpContext.Session.Clear();
			HttpContext.Session.Abandon();
			return RedirectToAction("Index");
		}

		[HttpPost]
		public JsonResult VericarSession()
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			try
			{
				if (WebSession.UserName == null || WebSession.Login == null)
				{
					respuesta.Mensaje = "Su sesión a caducado, se redireccionará al login para que ingrese sus credenciales";
					respuesta.Ok = false;
				}
			}
			catch (System.Exception ex)
			{
				_log.Error(ex, ex.Message);
				respuesta.Mensaje = ex.Message;
				respuesta.Ok = false;
			}
			return Json(respuesta);
		}
	}
}