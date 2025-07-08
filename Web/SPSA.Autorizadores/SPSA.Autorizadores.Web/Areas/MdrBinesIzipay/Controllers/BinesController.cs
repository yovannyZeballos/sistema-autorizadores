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
        public async Task<ActionResult> DescargarCsvStreaming(string nomEmpresa, string codEmpresa, string nomPeriodo, long codPeriodo)
        {
            if (string.IsNullOrWhiteSpace(codEmpresa))
            {
                return Json(new { ok = false, mensaje = "Seleccionar empresa es obligatorios." }, JsonRequestBehavior.AllowGet);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SGP"].ConnectionString;

            try
            {
                Response.Clear();
                Response.ContentType = "text/csv; charset=utf-8";
                string fileName = $"ConsolidadoBines_{nomEmpresa}_{nomPeriodo}.csv";
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                byte[] bom = Encoding.UTF8.GetPreamble();
                await Response.OutputStream.WriteAsync(bom, 0, bom.Length);

                string headerLine = "Empresa,Periodo,NomTarjeta,Marca,NumBin6,NumBin8,BancoEmisor,Tipo,Clasificacion,Mdr\r\n";
                byte[] headerBytes = Encoding.UTF8.GetBytes(headerLine);
                await Response.OutputStream.WriteAsync(headerBytes, 0, headerBytes.Length);

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    const string sql = @"SELECT * FROM ""SGP"".sf_mdr_consolidado_bines(@p_cod_empresa, @p_cod_periodo);";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("@p_cod_empresa", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codEmpresa });
                        cmd.Parameters.Add(new NpgsqlParameter("@p_cod_periodo", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = codPeriodo });
                        cmd.CommandTimeout = 0;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var sb = new StringBuilder();

                            while (await reader.ReadAsync())
                            {
                                // Función para escapar si hay comas o comillas
                                string Esc(string s)
                                {
                                    if (s.Contains(",") || s.Contains("\"") || s.Contains("\r") || s.Contains("\n"))
                                    {
                                        var temp = s.Replace("\"", "\"\"");
                                        return $"\"{temp}\"";
                                    }
                                    return s;
                                }

                                string nomEmpr = reader["NOM_EMPRESA"] != DBNull.Value ? reader["NOM_EMPRESA"].ToString() : "";
                                string numAnoR = reader["DES_PERIODO"] != DBNull.Value ? reader["DES_PERIODO"].ToString() : "";
                                string nomTarj = reader["NOM_TARJETA"] != DBNull.Value ? reader["NOM_TARJETA"].ToString() : "";
                                string nomOper = reader["NOM_OPERADOR"] != DBNull.Value ? reader["NOM_OPERADOR"].ToString() : "";
                                string numBin6 = reader["NUM_BIN_6"] != DBNull.Value ? reader["NUM_BIN_6"].ToString() : "";
                                string numBin8 = reader["NUM_BIN_8"] != DBNull.Value ? reader["NUM_BIN_8"].ToString() : "";
                                string bancoEm = reader["BANCO_EMISOR"] != DBNull.Value ? reader["BANCO_EMISOR"].ToString() : "";
                                string tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : "";
                                string nomClas = reader["NOM_CLASIFICACION"] != DBNull.Value ? reader["NOM_CLASIFICACION"].ToString() : "";
                                decimal factor = reader["FACTOR_MDR"] != DBNull.Value ? Convert.ToDecimal(reader["FACTOR_MDR"]) : 0m;

                                sb.Clear();
                                sb.Append(Esc(nomEmpr)).Append(",");
                                sb.Append(Esc(numAnoR)).Append(",");
                                sb.Append(Esc(nomTarj)).Append(",");
                                sb.Append(Esc(nomOper)).Append(",");
                                sb.Append(Esc(numBin6)).Append(",");
                                sb.Append(Esc(numBin8)).Append(",");
                                sb.Append(Esc(bancoEm)).Append(",");
                                sb.Append(Esc(tipo)).Append(",");
                                sb.Append(Esc(nomClas)).Append(",");
                                sb.Append(factor.ToString("F2")).Append("%\r\n");

                                byte[] lineBytes = Encoding.UTF8.GetBytes(sb.ToString());
                                await Response.OutputStream.WriteAsync(lineBytes, 0, lineBytes.Length);
                            }
                        }
                    }
                }

                Response.End();
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

            var command = new ImportarMdrTmpBinesIzipayCommand
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