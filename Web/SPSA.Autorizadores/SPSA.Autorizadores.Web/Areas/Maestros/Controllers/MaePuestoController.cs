using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.DTOs;
using SPSA.Autorizadores.Aplicacion.ViewModel;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.Maestros.Controllers
{
    public class MaePuestoController : Controller
    {
        private readonly IMediator _mediator;

        public MaePuestoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Maestros/MaePuesto
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<ActionResult> NuevoForm(MaeColaboradorExtDTO model)
        //{
        //    ListarEmpresasAsociadasQuery objListarEmpresas = new ListarEmpresasAsociadasQuery
        //    {
        //        CodUsuario = model.UsuAsociado,
        //        Busqueda = string.Empty
        //    };

        //    var empresas = await _mediator.Send(objListarEmpresas);

        //    ListarLocalesAsociadasPorEmpresaQuery objListarLocales = new ListarLocalesAsociadasPorEmpresaQuery
        //    {
        //        CodUsuario = model.UsuAsociado,
        //        CodEmpresa = model.CodEmpresa
        //    };


        //    var viewModel = new MaeColaboradorExtViewModel
        //    {
        //        ColaboradorExt = model,
        //        Empresas = empresas.Data
        //    };

        //    return PartialView("_NuevoForm", viewModel);
        //}

        //[HttpPost]
        //public async Task<ActionResult> ModificarForm(MaeColaboradorExtDTO model)
        //{
        //    ListarEmpresasAsociadasQuery objListarEmpresas = new ListarEmpresasAsociadasQuery();
        //    objListarEmpresas.CodUsuario = model.UsuAsociado;
        //    objListarEmpresas.Busqueda = string.Empty;

        //    var empresas = await _mediator.Send(objListarEmpresas);

        //    var viewModel = new MaeColaboradorExtViewModel
        //    {
        //        ColaboradorExt = model,
        //        Empresas = empresas.Data
        //    };

        //    return PartialView("_ModificarForm", viewModel);
        //}


        //[HttpPost]
        //public async Task<JsonResult> Obtener(ObtenerMaePuestoQuery request)
        //{
        //    var respuesta = await _mediator.Send(request);
        //    return Json(respuesta);
        //}

        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarMaePuestoQuery request)
        {

            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public async Task<JsonResult> CrearColaborador(CrearMaePuestoCommand command)
        //{
        //    var respuesta = await _mediator.Send(command);
        //    return Json(respuesta);
        //}

        [HttpPost]
        public async Task<JsonResult> ModificarPuesto(ActualizarMaePuestoCommand command)
        {
            var respuesta = await _mediator.Send(command);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<JsonResult> Importar()
        {
            var respuesta = new RespuestaComunExcelDTO();
            foreach (var fileKey in Request.Files)
            {
                HttpPostedFileBase archivo = Request.Files[fileKey.ToString()];
                if (archivo is null)
                {
                    respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Se encontraron algunos errores en el archivo";
                    respuesta.Errores.Add(new ErroresExcelDTO
                    {
                        Fila = 1,
                        Mensaje = "No se ha seleccionado ningun archivo."
                    });
                    return Json(respuesta);
                }
                else
                {
                    respuesta = await _mediator.Send(new ImportarMaeColaboradorExtCommand
                    {
                        Archivo = archivo,
                        Usuario = WebSession.Login,
                        JerarquiaOrganizacional = WebSession.JerarquiaOrganizacional
                    });
                }
            }

            return Json(respuesta);
        }

    }
}