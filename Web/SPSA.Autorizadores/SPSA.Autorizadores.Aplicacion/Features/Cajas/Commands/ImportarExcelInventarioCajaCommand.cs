using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
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
						FilterSheet = (tableReader, sheetIndex) => tableReader.Name.ToLower().Contains("plantilla"),
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


						var codActivos = await _contexto.RepositorioInvTipoActivo.Obtener()
							.AsNoTracking()
							.Select(x => x.CodActivo)
							.ToListAsync();

						var invCajasList = new List<InvCajas>();


						for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
						{
							try
							{
								DataRow row = ds.Tables[0].Rows[i];

								if (!int.TryParse(row[5].ToString(), out int numCaja))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = i + 2,
										Mensaje = $"Número de caja incorrecto: {row[5]}"
									});
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
										Fila = i + 2,
										Mensaje = $"Fecha Garantía incorrecto: {fechaGarantiaString}"
									});
									continue;
								}

								if (fechaInicioLisingString != "" && !DateTime.TryParseExact(fechaInicioLisingString, formatStrings, formatProvider, DateTimeStyles.None, out fechaInicioLising))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = i + 2,
										Mensaje = $"Fecha inicio Lising incorrecto: {fechaInicioLisingString}"
									});
									continue;
								}

								if (fechaFinLisingString != "" && !DateTime.TryParseExact(fechaFinLisingString, formatStrings, formatProvider, DateTimeStyles.None, out fechaFinLising))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = i + 2,
										Mensaje = $"Fecha fin Lising incorrecto: {fechaFinLisingString}"
									});
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

								if (!ValidarTamañoCampos(invCajas, respuesta.Errores, i + 2))
									continue;


								if (!ValidarJerarquiaOrganizacional(request.JerarquiaOrganizacional, invCajas, respuesta.Errores, i + 2))
									continue;

								if (!codActivos.Contains(invCajas.CodActivo))
								{
									respuesta.Errores.Add(new ErroresExcelDTO
									{
										Fila = i + 2,
										Mensaje = $"El código de activo ingresado no existe: {invCajas.CodActivo}"
									});
									continue;
								}

								invCajasList.Add(invCajas);

							}
							catch (Exception ex)
							{
                                _contexto.Rollback();

                                respuesta.Errores.Add(new ErroresExcelDTO
								{
									Fila = i + 2,
									Mensaje = ex.Message
								});
							}
						}

						var batchSize = 100;
						var invCajasExistentes = new List<InvCajas>();

						for (int i = 0; i < invCajasList.Count; i += batchSize)
						{
							var batch = invCajasList.Skip(i).Take(batchSize).ToList();
							var codigosUnicos = batch.Select(y => $"{y.CodEmpresa}{y.CodCadena}{y.CodRegion}{y.CodZona}{y.CodLocal}{y.NumCaja}{y.CodActivo}").ToList();

							var batchResult = await _contexto.RepositorioInvCajas.Obtener(x => codigosUnicos.Any(y => y == x.CodEmpresa + x.CodCadena + x.CodRegion + x.CodZona + x.CodLocal + x.NumCaja + x.CodActivo)).ToListAsync();

							invCajasExistentes.AddRange(batchResult);
						}

						foreach (var invCajas in invCajasList)
						{
							var invCajaExistente = invCajasExistentes.FirstOrDefault(x => x.CodEmpresa == invCajas.CodEmpresa && x.CodCadena == invCajas.CodCadena
							&& x.CodRegion == invCajas.CodRegion && x.CodZona == invCajas.CodZona && x.CodLocal == invCajas.CodLocal
							&& x.NumCaja == invCajas.NumCaja && x.CodActivo == invCajas.CodActivo);

							if (invCajaExistente != null)
							{
								ActualizarInvCaja(invCajas, invCajaExistente);
							}
							else
							{
								AgregarInvCaja(invCajas);
							}
						}

					}

					respuesta.Ok = respuesta.Errores.Count == 0;
					respuesta.Mensaje = respuesta.Ok ? "Archivo importado correctamente." : "archivo importado con algunos errores.";

					await _contexto.GuardarCambiosAsync();

				}
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
				_logger.Error(ex, respuesta.Mensaje);
			}


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

			return exito;
		}

		private void ActualizarInvCaja(InvCajas invCajas, InvCajas invCajaBD)
		{
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

		private bool ValidarTamañoCampos(InvCajas invCajas, List<ErroresExcelDTO> errores, int fila)
		{
			var exito = true;

			if (invCajas.CodEmpresa.Length > 2)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"El codigo de enpresa no debe tener más de 2 digitos"
				});
				exito = false;
			}

			if (invCajas.CodCadena.Length > 2)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"El codigo de cadena no debe tener más de 2 digitos"
				});
				exito = false;
			}

			if (invCajas.CodRegion.Length > 2)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"El codigo de región no debe tener más de 2 digitos"
				});
				exito = false;
			}

			if (invCajas.CodZona.Length > 3)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"El codigo de zona no debe tener más de 3 digitos"
				});
				exito = false;
			}

			if (invCajas.CodLocal.Length > 4)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = $"El codigo de local no debe tener más de 4 digitos"
				});
				exito = false;
			}

			if (invCajas.CodModelo.Length > 50)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "El modelo no debe tener más de 50 caracteres"
				});
				exito = false;
			}

			if (invCajas.CodSerie.Length > 50)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "La serie no debe tener más de 50 caracteres"
				});
				exito = false;
			}

			if (invCajas.NumAdenda.Length > 50)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "La adenda no debe tener más de 50 caracteres"
				});
				exito = false;
			}

			if (invCajas.TipEstado.Length > 1)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "El estado no debe tener más de 1 caracteres"
				});
				exito = false;
			}

			if (invCajas.TipProcesador.Length > 50)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "El procesador no debe tener más de 50 caracteres"
				});
				exito = false;
			}

			if (invCajas.Memoria.Length > 50)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "La memoria no debe tener más de 50 caracteres"
				});
				exito = false;
			}

			if (invCajas.DesSo.Length > 50)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "El sistema operativo no debe tener más de 50 caracteres"
				});
				exito = false;
			}

			if (invCajas.VerSo.Length > 20)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "La versión del sistema operativo no debe tener más de 20 caracteres"
				});
				exito = false;
			}

			if (invCajas.CapDisco.Length > 20)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "La capacidad del disco no debe tener más de 20 caracteres"
				});
				exito = false;
			}

			if (invCajas.TipDisco.Length > 20)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "El tipo de disco no debe tener más de 20 caracteres"
				});
				exito = false;
			}

			if (invCajas.DesPuertoBalanza.Length > 50)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "El puerto de la balanza no debe tener más de 50 caracteres"
				});
				exito = false;
			}

			if (invCajas.TipoCaja.Length > 100)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "El tipo de caja no debe tener más de 100 caracteres"
				});
				exito = false;
			}

			if (invCajas.Hostname.Length > 50)
			{
				errores.Add(new ErroresExcelDTO
				{
					Fila = fila,
					Mensaje = "El hostname no debe tener más de 50 caracteres"
				});
				exito = false;
			}

			return exito;
		}

	}
}
