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

        [HttpGet]
        public async Task<ActionResult> DescargarCsvStreaming(string codEmpresa, string numAno)
        {
            if (string.IsNullOrWhiteSpace(codEmpresa) || string.IsNullOrWhiteSpace(numAno))
            {
                return Json(new { ok = false, mensaje = "codEmpresa y numAno son obligatorios." }, JsonRequestBehavior.AllowGet);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SGP"].ConnectionString;

            try
            {
                Response.Clear();
                Response.ContentType = "text/csv; charset=utf-8";
                string fileName = $"ConsolidadoBines_{codEmpresa}_{numAno}.csv";
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                byte[] bom = Encoding.UTF8.GetPreamble();
                await Response.OutputStream.WriteAsync(bom, 0, bom.Length);

                string headerLine = "CodEmpresa,NumAno,NumBin6,NumBin8,NomTarjeta,BancoEmisor,Tipo,FactorMdr,CodOperador\r\n";
                byte[] headerBytes = Encoding.UTF8.GetBytes(headerLine);
                await Response.OutputStream.WriteAsync(headerBytes, 0, headerBytes.Length);

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    const string sql = @"SELECT * FROM ""SGP"".sf_mdr_consolidado_bines(@p_cod_empresa, @p_num_ano);";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("@p_cod_empresa", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codEmpresa });
                        cmd.Parameters.Add(new NpgsqlParameter("@p_num_ano", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = numAno });
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

                                string codEmp = reader["COD_EMPRESA"] != DBNull.Value ? reader["COD_EMPRESA"].ToString() : "";
                                string numAnoR = reader["NUM_ANO"] != DBNull.Value ? reader["NUM_ANO"].ToString() : "";
                                string numBin6 = reader["NUM_BIN_6"] != DBNull.Value ? reader["NUM_BIN_6"].ToString() : "";
                                string numBin8 = reader["NUM_BIN_8"] != DBNull.Value ? reader["NUM_BIN_8"].ToString() : "";
                                string nomTarj = reader["CLASIFICACION_NOMBRE"] != DBNull.Value ? reader["CLASIFICACION_NOMBRE"].ToString() : "";
                                string bancoEm = reader["BANCO_EMISOR"] != DBNull.Value ? reader["BANCO_EMISOR"].ToString() : "";
                                string tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : "";
                                decimal factor = reader["FACTOR"] != DBNull.Value ? Convert.ToDecimal(reader["FACTOR"]) : 0m;
                                string codOp = reader["COD_OPERADOR"] != DBNull.Value ? reader["COD_OPERADOR"].ToString() : "";

                                sb.Clear();
                                sb.Append(Esc(codEmp)).Append(",");
                                sb.Append(Esc(numAnoR)).Append(",");
                                sb.Append(Esc(numBin6)).Append(",");
                                sb.Append(Esc(numBin8)).Append(",");
                                sb.Append(Esc(nomTarj)).Append(",");
                                sb.Append(Esc(bancoEm)).Append(",");
                                sb.Append(Esc(tipo)).Append(",");
                                sb.Append(factor.ToString("F2")).Append(",");
                                sb.Append(Esc(codOp)).Append("\r\n");

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
        public async Task<JsonResult> DesdeExcel(HttpPostedFileBase archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.ContentLength == 0)
            {
                return Json(new { ok = false, mensaje = "Debes seleccionar un archivo .xlsx." });
            }

            var command = new ImportarMdrTmpBinesIzipayCommand
            {
                Archivo = archivoExcel
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