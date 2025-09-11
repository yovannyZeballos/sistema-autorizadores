using System.Data;
using System.Net.Sockets;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using SGP.Api.Models;
using SGP.Api.Services.SgpService;

namespace SGP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketGlpiController : ControllerBase
    {
        private readonly SgpService _SGPService;

        public TicketGlpiController(SgpService sgpService)
        {
            _SGPService = sgpService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No hay archivo");

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = file.OpenReadStream())
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true // Configura para usar la primera fila como encabezado
                            }
                        });

                        var dataTable = result.Tables[0];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (row[0] == DBNull.Value) continue; // Ignorar filas vacías

                            TicketGlpi ticket = new ()
                            {
                                Id = row[0].ToString(),
                                Titulo = row[1].ToString(),
                                Estado = row[2].ToString(),
                                EncuestaSatisfaccionFechaCreacion = row[3].ToString(),
                                Solicitante = row[4].ToString(),
                                Tipo = row[5].ToString(),
                                FechaApertura = DateTime.Parse(row[6].ToString()),
                                UltimaActualizacion = DateTime.Parse(row[7].ToString()),
                                Categoria = row[8].ToString(),
                                GrupoTecnicos = row[9].ToString(),
                                Tecnico = row[10].ToString(),
                                Localizaciones = row[11].ToString(),
                                FuentesSolicitantes = row[12].ToString(),
                                Prioridad = row[13].ToString(),
                                Entidad = row[14].ToString(),
                                FechaSolucion = row[15].ToString(),
                                ElementosAsociados = row[16].ToString(),
                                TiempoAdueñarse = row[17].ToString(),
                                TiempoSolucion = row[18].ToString(),
                                Autor = row[19].ToString(),
                                TiempoSolucionEstadisticas = row[20].ToString(),
                                TiempoAtenderServicio = row[21].ToString(),
                                IncidentesSociedad1 = row[22].ToString(),
                                SolicitudSociedad2 = row[23].ToString(),
                                IncidentesAplicacionPrograma = row[24].ToString(),
                                TiempoEspera = row[25].ToString(),
                                TipoTarea = row[26].ToString(),
                                GrupoSolicitante = row[27].ToString()
                            };

                            _SGPService.InsertarTickeGlpi(ticket);
                        }
                    }
                }

                return Ok("Archivo subido y la data fue ingresada");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
