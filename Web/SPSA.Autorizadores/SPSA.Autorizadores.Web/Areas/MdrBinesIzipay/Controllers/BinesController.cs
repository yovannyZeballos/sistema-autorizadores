using System.Text;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.Bines;
using SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Queries.Bines;
using Npgsql;
using System.Configuration;
using SPSA.Autorizadores.Aplicacion.DTO;
using System.Web;

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
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ListarBines(ListarMdrBinesIzipayQuery request)
        {
            var respuesta = await _mediator.Send(request);
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> DescargarCsvStreaming(string codEmpresa, string nomPeriodo, int codPeriodo)
        {
            if (string.IsNullOrWhiteSpace(codEmpresa))
            {
                return Json(new { ok = false, mensaje = "Seleccionar empresa es obligatorio." }, JsonRequestBehavior.AllowGet);
            }

            var connectionString = ConfigurationManager.ConnectionStrings["SGP"].ConnectionString;

            try
            {
                Response.Clear();
                Response.BufferOutput = false;
                Response.ContentType = "text/csv; charset=utf-8";
                var fileName = $"ConsolidadoBines_{(string.IsNullOrWhiteSpace(nomPeriodo) ? DateTime.Now.ToString("yyyyMMdd_HHmmss") : nomPeriodo)}.csv";
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                // BOM UTF-8
                var bom = Encoding.UTF8.GetPreamble();
                await Response.OutputStream.WriteAsync(bom, 0, bom.Length);

                // Encabezado (todas las columnas de mdr_bines_izipay)
                var headerLine = "periodo,empresa,bin6,bin8_9,marca,tipo_tarjeta,subproducto,banco_emisor,factor_mdr\r\n";
                var headerBytes = Encoding.UTF8.GetBytes(headerLine);
                await Response.OutputStream.WriteAsync(headerBytes, 0, headerBytes.Length);

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // Llama a la función que retorna todas las columnas en minúsculas
                    const string sql = @"SELECT * FROM ""SGP"".mdr_bines_sf_consolidado(@p_cod_empresa, @p_cod_periodo);";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("@p_cod_empresa", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codEmpresa });
                        cmd.Parameters.Add(new NpgsqlParameter("@p_cod_periodo", NpgsqlTypes.NpgsqlDbType.Integer) { Value = codPeriodo });
                        cmd.CommandTimeout = 0;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            string Esc(string s)
                            {
                                if (string.IsNullOrEmpty(s)) return "";
                                if (s.Contains(",") || s.Contains("\"") || s.Contains("\r") || s.Contains("\n"))
                                {
                                    var temp = s.Replace("\"", "\"\"");
                                    return $"\"{temp}\"";
                                }
                                return s;
                            }

                            var sb = new StringBuilder(256);
                            while (await reader.ReadAsync())
                            {
                                var desPeriodoStr = reader["des_periodo"] != DBNull.Value ? reader["des_periodo"].ToString() : "";
                                var nomEmpStr = reader["nom_empresa"] != DBNull.Value ? reader["nom_empresa"].ToString() : "";
                                var bin6Str = reader["bin_6"] != DBNull.Value ? reader["bin_6"].ToString() : "";
                                var bin89Str = reader["bin_8_9"] != DBNull.Value ? reader["bin_8_9"].ToString() : "";
                                var marcaStr = reader["marca"] != DBNull.Value ? reader["marca"].ToString() : "";
                                var tipoStr = reader["tipo_tarjeta"] != DBNull.Value ? reader["tipo_tarjeta"].ToString() : "";
                                var subprodStr = reader["subproducto"] != DBNull.Value ? reader["subproducto"].ToString() : "";
                                var bancoStr = reader["banco_emisor"] != DBNull.Value ? reader["banco_emisor"].ToString() : "";
                                var factor = reader["factor_mdr"] != DBNull.Value ? Convert.ToDecimal(reader["factor_mdr"]) : 0m;

                                sb.Clear();
                                sb.Append(Esc(desPeriodoStr)).Append(",");
                                sb.Append(Esc(nomEmpStr)).Append(",");
                                sb.Append(Esc(bin6Str)).Append(",");
                                sb.Append(Esc(bin89Str)).Append(",");
                                sb.Append(Esc(marcaStr)).Append(",");
                                sb.Append(Esc(tipoStr)).Append(",");
                                sb.Append(Esc(subprodStr)).Append(",");
                                sb.Append(Esc(bancoStr)).Append(",");
                                sb.Append(factor.ToString("F4")).Append("\r\n");

                                var lineBytes = Encoding.UTF8.GetBytes(sb.ToString());
                                await Response.OutputStream.WriteAsync(lineBytes, 0, lineBytes.Length);
                            }
                        }
                    }
                }

                await Response.OutputStream.FlushAsync();
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error al generar CSV: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> ImportarDesdeExcel(HttpPostedFileBase archivoExcel, int codPeriodo)
        {
            if (archivoExcel == null || archivoExcel.ContentLength == 0)
            {
                return Json(new { ok = false, mensaje = "Debes seleccionar un archivo .xlsx." });
            }

            var command = new ImportarMdrBinesInretailCommand
            {
                Archivo = archivoExcel,
                CodPeriodo = codPeriodo,
            };

            RespuestaComunExcelDTO resultado = await _mediator.Send(command);

            return Json(new
            {
                ok = resultado.Ok,
                mensaje = resultado.Mensaje,
                errores = resultado.Errores
            });
        }
    }
}