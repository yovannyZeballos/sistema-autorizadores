using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGP.Api.Models;
using SGP.Api.Services;
using SGP.Api.Services.BctService;
using SGP.Api.Services.BctService.DTOs;
using SGP.Api.Services.SgpService;
using SGP.Api.Services.SgpService.DTOs;

namespace SGP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalController(SgpService sgpService, SPT03Service spt03Service, BctSpsaService bctSpsaService) : ControllerBase
    {
        private readonly SgpService _sgpService = sgpService;
        private readonly SPT03Service _SPT03Service = spt03Service;
        private readonly BctSpsaService _BctSpsaService = bctSpsaService;

        [HttpGet("actualizar-locales-sgp")]
        public IActionResult ActualizarLocalesSgp()
        {
            try
            {
                List<EmpresaDto> listaEmpresas = _sgpService.ObtenerEmpresas();
                List<IRS_LOCALES> listaLocales = _SPT03Service.ObtenerLocalesCT2();
                foreach (var local in listaLocales)
                {
                    EmpresaDto empresa = listaEmpresas.Where(x => x.CodSociedad == local.LOC_SOCIEDAD).FirstOrDefault();
                    local.COD_EMPRESA = empresa.CodEmpresa;

                    List<CadenaDto> listaCadenas = _sgpService.ObtenerCadenas(local.COD_EMPRESA);

                    CadenaDto cadena = listaCadenas.Where(x => x.CadNumero == local.CAD_NUMERO).FirstOrDefault();

                    if (cadena == null)
                    {
                        local.COD_CADENA = listaCadenas.FirstOrDefault().CodCadena;
                    }
                    else
                    {
                        local.COD_CADENA = cadena.CodCadena;
                    }

                    LocalDto existeLocal = _sgpService.ObtenerLocal(local.COD_EMPRESA, local.COD_CADENA, local.LOC_NUMERO);

                    if (existeLocal == null)
                    {
                        Console.WriteLine($"Número de local: {local.LOC_NUMERO}, Descripción: {local.LOC_DESCRIPCION}");

                        if (local.COD_CADENA != "00")
                        {
                            string rpta = _sgpService.CrearLocal(local);

                            Console.WriteLine(rpta);
                        }
                        else
                        {
                            Console.WriteLine("Inserción fallida.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Número de local: {local.LOC_NUMERO}, Descripción: {local.LOC_DESCRIPCION}");
                        string rpta = _sgpService.ActualizarLocal(local);

                        Console.WriteLine(rpta);
                    }
                }
                return Ok("Se realizó actualizacion de locales en SGP correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("actualizar-locales-bct")]
        public IActionResult ActualizarLocalesBct()
        {
            try
            {
                List<LocalBctDTO> listaLocales = _sgpService.ObtenerLocalesParaBct();

                foreach (var local in listaLocales)
                {
                    LocalBctDTO existeLocal = _BctSpsaService.ObtenerLocal(local.RucEmisor, local.Codigo);
                    if (existeLocal == null)
                    {
                        Console.WriteLine($"Número de local: {local.Codigo}, Descripción: {local.Descripcion}");
                        string rpta = _BctSpsaService.CrearLocal(local);
                        Console.WriteLine(rpta);
                    }
                    else
                    {
                        Console.WriteLine($"Número de local: {local.Codigo}, Descripción: {local.Descripcion}");
                        string rpta = _BctSpsaService.ActualizarLocal(local);
                        Console.WriteLine(rpta);
                    }

                }
                return Ok("Se realizó actualizacion de locales en SGP correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
