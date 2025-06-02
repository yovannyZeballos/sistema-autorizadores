using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.FactoresMdr;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.FactoresMdr;
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
        /// Ejemplo: /FactoresMdr/ListarFactores?CodEmpresa=01&NumAno=2025
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> ListarFactores(ListarMdrFactorIzipayQuery request)
        {
            var respuesta = await _mediator.Send(request);
            // GenericResponseDTO<List<ListarMdrFactorDto>>
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

        /// <summary>
        /// (Opcional) Si quisieras eliminar o deshabilitar un factor MDR:
        /// [HttpPost] public async Task<JsonResult> EliminarFactor(EliminarMdrFactorIzipayCommand command) { ... }
        /// (No lo definimos en este ejemplo inicial.)
        /// </summary>
    }
}