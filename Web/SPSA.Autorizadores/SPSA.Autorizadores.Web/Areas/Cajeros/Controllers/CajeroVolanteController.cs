using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Cajeros.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Queries;
using SPSA.Autorizadores.Web.Models.Intercambio;
using SPSA.Autorizadores.Web.Utiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SPSA.Autorizadores.Web.Areas.Cajeros.Controllers
{
	public class CajeroVolanteController : Controller
	{

		private readonly IMediator _mediator;

		public CajeroVolanteController(IMediator mediator)
		{
			_mediator = mediator;
		}

		// GET: Cajeros/CajeroVolante
		public ActionResult Index()
		{
			return View();
		}

        [HttpPost]
        public async Task<JsonResult> ListarCajerosVolante(DataTableRequest request)
        {
            var pageNumber = (request.start / request.length) + 1;
            var pageSize = request.length;
            var searchValue = request.search?.value;

            var response = await _mediator.Send(new ListarCajeroVolanteQuery
            {
                CodEmpresa = WebSession.CodigoEmpresa,
                CodCoordinador = WebSession.Login,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = searchValue
            });

            return Json(new
            {
                draw = request.draw,
                recordsTotal = response.TotalRegistros,
                recordsFiltered = response.TotalFiltrados,
                columns = response.Columnas,   // 🔹 columnas dinámicas
                data = response.Data           // 🔹 filas dinámicas
            });
        }

        [HttpPost]
		public async Task<JsonResult> ListarCajerosVolanteOfiplan()
		{
			var response = await _mediator.Send(new ListarCajeroVolanteOfiplanQuery { CodEmpresa = WebSession.CodigoEmpresa, CodLocal = Convert.ToDecimal(WebSession.Local) });
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> CrearCajerosVolante(List<CrearCajeroVolanteCommand> request)
		{
			var response = new RespuestaComunDTO() { Ok = true };
			var sb = new StringBuilder();
			foreach (var item in request)
			{
				item.CodEmpresaOrigen = WebSession.CodigoEmpresa;
				item.CodEmpresa = WebSession.CodigoEmpresa;
				//item.LocalesAsignados = WebSession.LocalesAsignadosXEmpresa.Select(x => x.Codigo).ToList();
				item.Coordinador = WebSession.Login;
				item.Usuario = WebSession.Login;
				var rpta = await _mediator.Send(item);
				if (!rpta.Ok)
				{
					sb.AppendLine(rpta.Mensaje);
					response.Ok = false;
				}
			}

			response.Mensaje = sb.ToString();
			return Json(response);
		}

		[HttpPost]
		public async Task<JsonResult> EliminarCajerosVolante(List<EliminarCajeroVolanteCommand> request)
		{
			var response = new RespuestaComunDTO() { Ok = true };
			var sb = new StringBuilder();

			foreach (var item in request)
			{
				item.Usuario = WebSession.Login;
				item.CodEmpresa = WebSession.CodigoEmpresa;
				var rpta = await _mediator.Send(item);
				if (!rpta.Ok)
				{
					sb.AppendLine(rpta.Mensaje);
					response.Ok = false;
				}
			}

			response.Mensaje = sb.ToString();
			return Json(response);
		}
	}
}