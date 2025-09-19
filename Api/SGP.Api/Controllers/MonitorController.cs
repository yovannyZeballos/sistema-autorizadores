using Microsoft.AspNetCore.Mvc;
using SGP.Api.Models;
using SGP.Api.Services.BctService;
using SGP.Api.Services.SgpService;
using SGP.Api.Services;
using SGP.Api.Services.Ct3Service;

namespace SGP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitorController(SgpService sgpService, SPT03Service spt03Service, HPCT02Service hpct02Service, TPCT02Service tpct02Service,
        BctSpsaService bctSpsaService, BctTpsaService bctTpsaService, BctHpsaService bctHpsaService, Ct3SpsaService ct3SpsaService) : ControllerBase
    {
        private readonly SgpService _sgpService = sgpService;
        private readonly SPT03Service _SPT03Service = spt03Service;
        private readonly HPCT02Service _HPCT02Service = hpct02Service;
        private readonly TPCT02Service _TPCT02Service = tpct02Service;
        private readonly BctSpsaService _bctSpsaService = bctSpsaService;
        private readonly BctTpsaService _bctTpsaService = bctTpsaService;
        private readonly BctHpsaService _bctHpsaService = bctHpsaService;

        private readonly Ct3SpsaService _ct3SpsaService = ct3SpsaService;

        [HttpGet("actualizar-fecha-negocio")]
        public async Task<IActionResult> ActualizarFechaNegocio()
        {
            try
            {
                //string fechaSP = await _SPT03Service.ObtenerFechaNegocio();
                string fechaSP = await _ct3SpsaService.ObtenerFechaNegocio();
                string respuestaSP = _sgpService.AcualizarParametroFechaNegocio(fechaSP, "02");
                string fechaTP = await _TPCT02Service.ObtenerFechaNegocio();
                string respuestaTP = _sgpService.AcualizarParametroFechaNegocio(fechaTP, "09");

                string respuestaHP = string.Empty;
                try
                {
                    string fechaHP = await _HPCT02Service.ObtenerFechaNegocio();
                    respuestaHP = _sgpService.AcualizarParametroFechaNegocio(fechaHP, "10");
                }
                catch
                {
                    respuestaHP = " Error";
                }

                return Ok("SP: " + respuestaSP + " | " + "TP: " + respuestaTP + " | " + "HP: " + respuestaHP);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("actualizar-estado-conexion")]
        public async Task<IActionResult> ActualizarEstadoConexion()
        {
            try
            {
                string estadoConnSP = await _ct3SpsaService.ObtenerEstadoConexion();
                string estadoConnTP = await _TPCT02Service.ObtenerEstadoConexion();
                string estadoConnHP = await _HPCT02Service.ObtenerEstadoConexion();

                string respuestaSP = _sgpService.AcualizarParametroEstadoConexion(estadoConnSP, "02");
                string respuestaTP = _sgpService.AcualizarParametroEstadoConexion(estadoConnTP, "09");
                string respuestaHP = _sgpService.AcualizarParametroEstadoConexion(estadoConnHP, "10");


                return Ok("SP: " + respuestaSP + " | " + "TP: " + respuestaTP + " | " + "HP: " + respuestaHP);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("integrador-bct-ct")]
        public async Task<IActionResult> IntegradorMonitorBctCt()
        {
            try
            {
                DateTime fechaEjecucion = DateTime.Now;
                ProcesoParamPorEmpresa? parametroSpsa = await _sgpService.ObtenerParametro("02", 29, "02");

                if (parametroSpsa == null || parametroSpsa.ValParametro == null)
                {
                    return StatusCode(500, "El parámetro obtenido es nulo.");
                }

                var monitorSpsa = await _bctSpsaService.ObtenerMonitorBct();
                monitorSpsa.CodEmpresa = "02";
                monitorSpsa.Limite = Convert.ToInt32(parametroSpsa.ValParametro);
                monitorSpsa.FechaHora = fechaEjecucion;
                monitorSpsa.Fecha = monitorSpsa.FechaHora;
                monitorSpsa.Hora = Convert.ToInt32(monitorSpsa.FechaHora.ToString("HH"));
                monitorSpsa.Minuto = Convert.ToInt32(monitorSpsa.FechaHora.ToString("mm"));

                _ = await _sgpService.RegistrarMonitorBctCt(monitorSpsa);

                ProcesoParamPorEmpresa? parametroTpsa = await _sgpService.ObtenerParametro("09", 29, "02");

                if (parametroTpsa == null || parametroTpsa.ValParametro == null)
                {
                    return StatusCode(500, "El parámetro obtenido es nulo.");
                }

                var monitorTpsa = await _bctTpsaService.ObtenerMonitorBct();
                monitorTpsa.CodEmpresa = "09";
                monitorTpsa.Limite = Convert.ToInt32(parametroTpsa.ValParametro);
                monitorTpsa.FechaHora = fechaEjecucion;
                monitorTpsa.Fecha = monitorSpsa.FechaHora;
                monitorTpsa.Hora = Convert.ToInt32(monitorSpsa.FechaHora.ToString("HH"));
                monitorTpsa.Minuto = Convert.ToInt32(monitorSpsa.FechaHora.ToString("mm"));

                _ = await _sgpService.RegistrarMonitorBctCt(monitorTpsa);

                ProcesoParamPorEmpresa? parametroHpsa = await _sgpService.ObtenerParametro("10", 29, "02");

                if (parametroHpsa == null || parametroHpsa.ValParametro == null)
                {
                    return StatusCode(500, "El parámetro obtenido es nulo.");
                }

                var monitorHpsa = await _bctHpsaService.ObtenerMonitorBct();
                monitorHpsa.CodEmpresa = "10";
                monitorHpsa.Limite = Convert.ToInt32(parametroHpsa.ValParametro);
                monitorHpsa.FechaHora = fechaEjecucion;
                monitorHpsa.Fecha = monitorSpsa.FechaHora;
                monitorHpsa.Hora = Convert.ToInt32(monitorSpsa.FechaHora.ToString("HH"));
                monitorHpsa.Minuto = Convert.ToInt32(monitorSpsa.FechaHora.ToString("mm"));

                _ = await _sgpService.RegistrarMonitorBctCt(monitorHpsa);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
