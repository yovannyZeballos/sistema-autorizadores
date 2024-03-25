using DocumentFormat.OpenXml;
using ExcelDataReader;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Extensiones;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands
{
	public class ImportarExcelInventarioCajaCommand : IRequest<RespuestaComunExcelDTO>
	{
		public HttpPostedFileBase Archivo { get; set; }
		public string Usuario { get; set; }
		public JerarquiaOrganizacionalDTO JerarquiaOrganizacional { get; set; }
	}

	public class ImportarExcelInventarioCajaHandler : IRequestHandler<ImportarExcelInventarioCajaCommand, RespuestaComunExcelDTO>
	{

		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		public ImportarExcelInventarioCajaHandler()
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		public async Task<RespuestaComunExcelDTO> Handle(ImportarExcelInventarioCajaCommand request, CancellationToken cancellationToken)
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
						var formatStrings = new string[] { "dd/MM/yyyy HH:mm:ss", "d/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss", "d-MM-yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "d/MM/yyyy H:mm:ss", "dd-MM-yyyy H:mm:ss", "d-MM-yyyy H:mm:ss" };

						var fila = 1;
						foreach (DataRow row in ds.Tables[0].Rows)
						{
							try
							{
								if (!int.TryParse(row[5].ToString(), out int numCaja))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = fila,
										Mensaje = $"Número de caja incorrecto: {row[5]}"
									});
									fila++;
									continue;
								}

								var fechaGarantiaString = row[15].ToString() ?? "";
								var fechaInicioLisingString = row[24].ToString() ?? "";
								var fechaFinLisingString = row[25].ToString() ?? "";

								DateTime fecGarantia = DateTime.MinValue;
								DateTime fechaInicioLising = DateTime.MinValue;
								DateTime fechaFinLising = DateTime.MinValue;

								if (fechaGarantiaString != "" && !DateTime.TryParseExact(fechaGarantiaString, formatStrings, formatProvider, DateTimeStyles.None, out fecGarantia))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = fila,
										Mensaje = $"Fecha Garantía incorrecto: {fechaGarantiaString}"
									});
									fila++;
									continue;
								}

								if (fechaInicioLisingString != "" && !DateTime.TryParseExact(fechaInicioLisingString, formatStrings, formatProvider, DateTimeStyles.None, out fechaInicioLising))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = fila,
										Mensaje = $"Fecha inicio Lising incorrecto: {fechaInicioLisingString}"
									});
									fila++;
									continue;
								}

								if (fechaFinLisingString != "" && !DateTime.TryParseExact(fechaFinLisingString, formatStrings, formatProvider, DateTimeStyles.None, out fechaFinLising))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = fila,
										Mensaje = $"Fecha fin Lising incorrecto: {fechaFinLisingString}"
									});
									fila++;
									continue;
								}


								var invCajas = new InvCajas
								{
									CodEmpresa = row[0].ToString(),
									CodCadena = row[1].ToString(),
									CodRegion = row[2].ToString(),
									CodZona = row[3].ToString(),
									CodLocal = row[4].ToString(),
									NumCaja = numCaja,
									CodActivo = row[10].ToString(),
									CodModelo = row[12].ToString(),
									CodSerie = row[13].ToString(),
									NumAdenda = row[14].ToString(),
									FecGarantia = fecGarantia == DateTime.MinValue ? null : (DateTime?)fecGarantia,
									TipEstado = row[16].ToString(),
									TipProcesador = row[17].ToString(),
									Memoria = row[18].ToString(),
									DesSo = row[19].ToString(),
									VerSo = row[20].ToString(),
									CapDisco = row[21].ToString(),
									TipDisco = row[22].ToString(),
									DesPuertoBalanza = row[23].ToString(),
									TipoCaja = row[8].ToString(),
									Hostname = row[9].ToString(),
									FechaInicioLising = fechaInicioLising == DateTime.MinValue ? null : (DateTime?)fechaInicioLising,
									FechaFinLising = fechaFinLising == DateTime.MinValue ? null : (DateTime?)fechaFinLising,
								};

								if (!ValidarJerarquiaOrganizacional(request.JerarquiaOrganizacional, invCajas, respuesta.Errores, fila))
								{
									fila++;
									continue;
								}

								if (!await ExisteTipoActivo(invCajas.CodActivo))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = fila,
										Mensaje = $"El código de activo ingresado no existe: {invCajas.CodActivo}"
									});
									fila++;
									continue;
								}

								if (await ExisteInvCajas(invCajas))
								{
									await ActualizarInvCaja(invCajas);
								}
								else
								{
									AgregarInvCaja(invCajas);
								}


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
				_logger.Error(ex, respuesta.Mensaje);
			}

			await _contexto.GuardarCambiosAsync();

			return respuesta;
		}

		private bool ValidarJerarquiaOrganizacional(JerarquiaOrganizacionalDTO jerarquiaOrganizacional, InvCajas invCajas, List<ErroresExcelDTO> errores, int fila)
		{
			var exito = true;

			if (!jerarquiaOrganizacional.EmpresasAsociadas.Any(x => x.CodEmpresa == invCajas.CodEmpresa))
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"Empresa {invCajas.CodEmpresa} no asociada al usuario"
				});

				exito = false;
			}


			if (!jerarquiaOrganizacional.CadenasAsociadas.Any(x => x.CodEmpresa == invCajas.CodEmpresa && x.CodCadena == invCajas.CodCadena))
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"Cadena {invCajas.CodCadena} no asociada al usuario"
				});
				exito = false;
			}


			if (!jerarquiaOrganizacional.RegionesAsociadas.Any(x => x.CodEmpresa == invCajas.CodEmpresa && x.CodCadena == invCajas.CodCadena && x.CodRegion == invCajas.CodRegion))
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"Región {invCajas.CodRegion} no asociada al usuario"
				});
				exito = false;
			}


			if (!jerarquiaOrganizacional.ZonasAsociadas.Any(x => x.CodEmpresa == invCajas.CodEmpresa && x.CodCadena == invCajas.CodCadena && x.CodRegion == invCajas.CodRegion && x.CodZona == invCajas.CodZona))
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"Zona {invCajas.CodZona} no asociada al usuario"
				});
				exito = false;
			}


			if (!jerarquiaOrganizacional.LocalesAsociados.Any(x => x.CodEmpresa == invCajas.CodEmpresa && x.CodCadena == invCajas.CodCadena && x.CodRegion == invCajas.CodRegion && x.CodZona == invCajas.CodZona && x.CodLocal == invCajas.CodLocal))
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"Local {invCajas.CodLocal} no asociado al usuario"
				});
				exito = false;
			}

			return exito;
		}

		private async Task<bool> ExisteTipoActivo(string codActivo)
		{
			return await _contexto.RepositorioInvTipoActivo.Existe(x => x.CodActivo == codActivo);
		}

		private async Task<bool> ExisteInvCajas(InvCajas invCajas)
		{
			return await _contexto.RepositorioInvCajas.Existe(x => x.CodEmpresa == invCajas.CodEmpresa && x.CodCadena == invCajas.CodCadena
			&& x.CodRegion == invCajas.CodRegion && x.CodZona == invCajas.CodZona && x.CodLocal == invCajas.CodLocal
			&& x.NumCaja == invCajas.NumCaja && x.CodActivo == invCajas.CodActivo);
		}


		private async Task ActualizarInvCaja(InvCajas invCajas)
		{
			var invCajaBD = await _contexto.RepositorioInvCajas.Obtener(x => x.CodEmpresa == invCajas.CodEmpresa && x.CodCadena == invCajas.CodCadena
			&& x.CodRegion == invCajas.CodRegion && x.CodZona == invCajas.CodZona && x.CodLocal == invCajas.CodLocal
			&& x.NumCaja == invCajas.NumCaja && x.CodActivo == invCajas.CodActivo).FirstOrDefaultAsync();

			invCajaBD.CapDisco = invCajas.CapDisco;
			invCajaBD.CodModelo = invCajas.CodModelo;
			invCajaBD.CodSerie = invCajas.CodSerie;
			invCajaBD.DesPuertoBalanza = invCajas.DesPuertoBalanza;
			invCajaBD.FecGarantia = invCajas.FecGarantia;
			invCajaBD.Memoria = invCajas.Memoria;
			invCajaBD.TipDisco = invCajas.TipDisco;
			invCajaBD.TipEstado = invCajas.TipEstado;
			invCajaBD.TipProcesador = invCajas.TipProcesador;
			invCajaBD.VerSo = invCajas.VerSo;
			invCajaBD.DesSo = invCajas.DesSo;
			invCajaBD.NumAdenda = invCajas.NumAdenda;
			invCajaBD.TipoCaja = invCajas.TipoCaja;
			invCajaBD.Hostname = invCajas.Hostname;
			invCajaBD.FechaInicioLising = invCajas.FechaInicioLising;
			invCajaBD.FechaFinLising = invCajas.FechaFinLising;
		}

		private void AgregarInvCaja(InvCajas invCajas)
		{
			_contexto.RepositorioInvCajas.Agregar(invCajas);
		}

	}
}
