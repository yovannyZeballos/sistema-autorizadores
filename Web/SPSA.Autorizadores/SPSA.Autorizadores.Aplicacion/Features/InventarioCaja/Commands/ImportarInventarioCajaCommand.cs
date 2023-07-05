using ExcelDataReader;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Web;
using SPSA.Autorizadores.Aplicacion.Extensiones;
using System.Globalization;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
	public class ImportarInventarioCajaCommand : IRequest<RespuestaComunExcelDTO>
	{
		public HttpPostedFileBase Archivo { get; set; }
		public string Usuario { get; set; }
	}

	public class ImportarInventarioCajaHandler : IRequestHandler<ImportarInventarioCajaCommand, RespuestaComunExcelDTO>
	{
		private readonly IRepositorioSovosInventarioCaja _repositorioSovosInventarioCaja;

		public ImportarInventarioCajaHandler(IRepositorioSovosInventarioCaja repositorioSovosInventarioCaja)
		{
			_repositorioSovosInventarioCaja = repositorioSovosInventarioCaja;
		}

		public async Task<RespuestaComunExcelDTO> Handle(ImportarInventarioCajaCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
			try
			{
				using (var reader = ExcelReaderFactory.CreateReader(request.Archivo.InputStream))
				{
					var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
					{
						FilterSheet = (tableReader, sheetIndex) =>
						{
							var name = tableReader.Name.ToLower();
							if (name.Contains("plantilla"))
								return true;
							else
								return false;
						},
						ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
						{
							UseHeaderRow = true,
						},
						UseColumnDataType = true
					}).ToAllStringFields();


					if (ds.Tables.Count > 0)
					{
						IFormatProvider formatProvider = CultureInfo.InvariantCulture;
						var fila = 1;
						foreach (DataRow row in ds.Tables[0].Rows)
						{
							try
							{
								var sovosCajaInventario = new SovosCajaInventario
								(
									row["COD_EMPRESA"].ToString(),
									row["COD_FORMATO"].ToString(),
									row["COD_LOCAL"].ToString(),
									Convert.ToInt32(row["NUM_POS"].ToString()),
									row["RANKING"].ToString(),
									row["ESTADO"].ToString(),
									row["SEDE"].ToString(),
									row["UBICACION"].ToString(),
									row["CAJA"].ToString(),
									row["MODELO_CPU"].ToString(),
									row["SERIE"].ToString(),
									row["MODELO_PRINT"].ToString(),
									row["SERIE_PRINT"].ToString(),
									row["MODELO_DYNAKEY"].ToString(),
									row["SERIE_DYNAKEY"].ToString(),
									row["MODELO_SCANNER"].ToString(),
									row["SERIE_SCANNER"].ToString(),
									row["MODELO_GAVETA"].ToString(),
									row["SERIE_GAVETA"].ToString(),
									row["MODELO_MONITOR"].ToString(),
									row["SERIE_MONITOR"].ToString(),
									string.IsNullOrEmpty(row["FECHA_APERTURA"].ToString()) ? null : (DateTime?)DateTime.ParseExact(row["FECHA_APERTURA"].ToString(), "dd-MM-yyyy", formatProvider),
									row["CARACT_1"].ToString(),
									row["CARACT_2"].ToString(),
									row["CARACT_3"].ToString(),
									string.IsNullOrEmpty(row["FECHA_LISING"].ToString()) ? null : (DateTime?)DateTime.ParseExact(row["FECHA_LISING"].ToString(), "dd-MM-yyyy", formatProvider),
									row["SO"].ToString(),
									row["VERSION_SO"].ToString(),
									string.IsNullOrEmpty(row["FECHA_ASIGNACION"].ToString()) ? null : (DateTime?)DateTime.ParseExact(row["FECHA_ASIGNACION"].ToString(), "dd-MM-yyyy", formatProvider),
									request.Usuario
								);

								await _repositorioSovosInventarioCaja.Insertar(sovosCajaInventario);
							}
							catch (Exception ex)
							{

								respuesta.Errores.Add(new ErroresExcelDTO
								{
									Fila = fila,
									Mensaje = ex.Message
								});
							}
							fila++;
						}
					}

					if (respuesta.Errores.Count == 0)
					{
						respuesta.Ok = true;
						respuesta.Mensaje = "Archivo importado correctamente";
					}
					else
					{
						respuesta.Ok = false;
						respuesta.Mensaje = "Se encontraron algunos errores en el archivo";
					}
					
				}
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return respuesta;
		}
	}
}
