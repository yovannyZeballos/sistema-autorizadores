using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.ClasificacionMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.OperadorMdr;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.MdrBinesIzipay.Controllers
{
    public class FactoresMdrController : Controller
    {
        private readonly IMediator _mediator;

        public FactoresMdrController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: MdrBinesIzipay/FactoresMdr
        public ActionResult Index()
        {
            // Vista que mostrará la tabla de Factores MDR y el formulario para crear uno nuevo.
            // Por ejemplo: Views/FactoresMdr/Index.cshtml
            return View();
        }

        /// <summary>
        /// Lista todos los Factores MDR para una empresa y año dados.
        /// Recibe por querystring: CodEmpresa y NumAno
        /// Ejemplo: /FactoresMdr/ListarPaginado?CodEmpresa=01&NumAno=2025
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> ListarPaginado(ListarMdrFactorIzipayQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Crea un nuevo Factor MDR.
        /// Body del POST: JSON con propiedades de CrearMdrFactorIzipayCommand.
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CrearFactorMdr(CrearMdrFactorIzipayCommand command)
        {
            command.UsuCreacion = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> EliminarFactorMdr(EliminarMdrFactorIzipayCommand command)
        {
            command.UsuElimina = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<JsonResult> ListarOperador(ListarMdrOperadorQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ListarClasificacion(ListarMdrClasificacionQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}