using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.Bines;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.Bines;
using SPSA.Autorizadores.Web.Utiles;

namespace SPSA.Autorizadores.Web.Areas.MdrBinesIzipay.Controllers
{
    public class BinesController : Controller
    {
        private readonly IMediator _mediator;

        public BinesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: MdrBinesIzipay/Bines
        public ActionResult Index()
        {
            // Aquí deberías devolver la vista que contiene el DataTable o formulario
            // para listar y crear Bines. Por ejemplo: Views/Bines/Index.cshtml
            return View();
        }

        /// <summary>
        /// Lista todos los Bines para una empresa y año dados.
        /// Espera recibir por querystring: CodEmpresa y NumAno
        /// Ejemplo de URL: /Bines/ListarBines?CodEmpresa=01&NumAno=2025
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> ListarBines(ListarMdrBinesIzipayQuery request)
        {
            var respuesta = await _mediator.Send(request);
            // GenericResponseDTO&lt;List&lt;ListarMdrBinesDto&gt;&gt;
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Crea un nuevo registro de BIN.
        /// Se espera que el body del POST tenga todas las propiedades de CrearMdrBinesIzipayCommand en JSON.
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CrearBine(CrearMdrBinesIzipayCommand command)
        {
            //command = WebSession.Login;
            var respuesta = await _mediator.Send(command);
            return Json(respuesta);
        }

        ///// <summary>
        ///// Devuelve un archivo CSV con el consolidado de Bines + Factores MDR
        ///// Se espera recibir por querystring: CodEmpresa y NumAno
        ///// Ejemplo de URL: /Bines/ConsolidarBines?CodEmpresa=01&NumAno=2025
        ///// </summary>
        //[HttpGet]
        //public async Task<ActionResult> ConsolidarBines(string CodEmpresa, string NumAno)
        //{
        //    // Construimos el query
        //    var query = new ConsolidarBinesQuery
        //    {
        //        CodEmpresa = CodEmpresa,
        //        NumAno = NumAno
        //    };

        //    // Obtenemos la lista de ConsolidadosBinesDto
        //    var resultado = await _mediator.Send(query);
        //    if (!resultado.Ok)
        //    {
        //        // Si hay error, devolvemos un JSON con el mensaje
        //        return Json(resultado, JsonRequestBehavior.AllowGet);
        //    }

        //    // Aquí debes convertir resultado.Data (List<ConsolidadosBinesDto>) en CSV (en bytes)
        //    // Por simplicidad, asumimos que el Handler ya construyó un CSV en Base64 en RespuestaComunDTO.Mensaje
        //    // (Si no, genera el CSV tal como mostramos en ejemplos anteriores.)
        //    var csvBytes = System.Text.Encoding.UTF8.GetBytes(resultado.Mensaje);

        //    var fileName = $"Consolidado_Bines_{CodEmpresa}_{NumAno}.csv";
        //    return File(csvBytes, "text/csv", fileName);
        //}
    }
}