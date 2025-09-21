using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGP.Api.Controllers.Request;
using SGP.Api.Services.CenService;

namespace SGP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteCenController(CenService cenService) : ControllerBase
    {
		private readonly CenService _cenService = cenService;


		[HttpGet("obtener")]
		public async Task<IActionResult> Obtener([FromQuery] ConsultaClienteCenRequest request)
		{
			try
			{
				var cliente = await _cenService.ObtenreCliente(request);
				if (cliente == null)
					return NotFound(new ErrorRequest { Mensaje = "Cliente no encontrado" });
				return Ok(cliente);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new ErrorRequest { Mensaje = $"Error interno del servidor: {ex.Message}" });
			}
			
		}

		[HttpPost("crear")]
		public async Task<IActionResult> Crear([FromBody] InsertarClienteCenRequest request)
		{
			try
			{
				int respuesta = await _cenService.InsertarCliente(request);

				if (respuesta == 3)
				{
					return StatusCode(500, new ErrorRequest { Mensaje = "No se pudo crear el cliente en CEN" });
				}

				return Created();
			}
			catch (Exception ex)
			{
				return StatusCode(500, new ErrorRequest { Mensaje = $"Error interno del servidor: {ex.Message}" });
			}

		}

	}
}
