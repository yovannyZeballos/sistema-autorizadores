using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
	public class ProcesarMonitorLocalBCTCommand : IRequest<GenericResponseDTO<List<ProcesarMonitorLocalBCTDTO>>>
	{
		public DateTime Fecha { get; set; }
		public List<string> Empresas { get; set; }
	}

	public class ProcesarMonitorLocalBCTHandler : IRequestHandler<ProcesarMonitorLocalBCTCommand, GenericResponseDTO<List<ProcesarMonitorLocalBCTDTO>>>
	{
		private readonly ILogger _logger;
		private List<ProcesoParametroEmpresa> procesoParametroEmpresas;
		private readonly IRepositorioElectronicJournal _repositorioElectronicJournal;
		private readonly IRepositorioTrxHeader _repositorioTrxHeader;

		public ProcesarMonitorLocalBCTHandler(IRepositorioElectronicJournal repositorioElectronicJournal, IRepositorioTrxHeader repositorioTrxHeader)
		{
			_logger = SerilogClass._log;
			_repositorioElectronicJournal = repositorioElectronicJournal;
			_repositorioTrxHeader = repositorioTrxHeader;
		}

		public async Task<GenericResponseDTO<List<ProcesarMonitorLocalBCTDTO>>> Handle(ProcesarMonitorLocalBCTCommand request, CancellationToken cancellationToken)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var respuesta = new GenericResponseDTO<List<ProcesarMonitorLocalBCTDTO>> { Ok = true };

			var fecha = request.Fecha.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
			ProcesoParametro procesoParametro = null;

			_logger.Information($"Iniciando proceso Monitor Diferencia de transacciones y montos, fecha: {fecha}");

			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					if (!await VerificarExistenciaProceso(contexto))
					{
						respuesta.Ok = false;
						respuesta.Mensaje = "No existe proceso o esta inactivo: Monitor Diferencia de transacciones y montos";
						_logger.Information(respuesta.Mensaje);
						return respuesta;
					}

					procesoParametro = await contexto.RepositorioProcesoParametro.Obtener(x => x.CodProceso == Constantes.CodigoProcesoDiferenciaTransacciones && x.CodParametro == "01").FirstOrDefaultAsync();

					var empresas = await ObtenerEmpresasActivas(contexto, request.Empresas);
					respuesta.Data = await ObtenerLocalesDeEmpresas(contexto, empresas);
					procesoParametroEmpresas = await contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoDiferenciaTransacciones && x.IndActivo == "S").ToListAsync();

				}
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = "Ocurrio un error al obetener los locales";
				_logger.Error(ex, respuesta.Mensaje);
				return respuesta;
			}


			//Recorrer BD POS SERVER
			var batchSize = 10;
			for (int i = 0; i < respuesta.Data.Count; i += batchSize)
			{
				var batch = respuesta.Data.Skip(i).Take(batchSize).ToList();

				var tasks = batch.Select(async localPos =>
				{
					var localDescripcion = $"{localPos.CodLocal}-{localPos.NomLocal}";

					try
					{
						if (string.IsNullOrEmpty(localPos.IP))
						{
							localPos.Observacion = $"{localDescripcion}: el local no tiene IP";
							localPos.Estado = "Error";
							localPos.ColorEstado = "ROJO";
							_logger.Information(localPos.Observacion);
							return;
						}

						var cadenaConexion = ArmarCadenaConexion(localPos);
						var reintentos = procesoParametro.ValParametro == null ? 1 : Convert.ToInt32(procesoParametro.ValParametro);
						var transacciones = await EjecutarConReintento(() => _repositorioElectronicJournal.ListarTransacciones(cadenaConexion, fecha), reintentos);
						localPos.CantTransaccionesLocal = transacciones.Count();
						localPos.NontoTransaccionesLocal = 0;

						_logger.Information($"{localDescripcion}: conexion exitosa, cantidad transacciones {localPos.CantTransaccionesLocal}");

						foreach (var transaccion in transacciones)
						{
							var data = transaccion.TrxData;

							var valorTotal = ExtraerValorDeParametro(data, "T O T A L");
							var valorPeruChamps = ExtraerValorDeParametro(data, "PERU CHAMPS");
							var valorTeleton = ExtraerValorDeParametro(data, "TELETON");
							var valorExcRedondeo = ExtraerValorDeParametro(data, "EXC.REDONDEO");


							if (valorTotal.HasValue)
							{
								localPos.NontoTransaccionesLocal += valorTotal.Value;
								if (valorPeruChamps.HasValue)
								{
									localPos.NontoTransaccionesLocal += valorPeruChamps.Value;
								}
								if (valorTeleton.HasValue)
								{
									localPos.NontoTransaccionesLocal += valorTeleton.Value;
								}
								if (valorExcRedondeo.HasValue)
								{
									localPos.NontoTransaccionesLocal -= valorExcRedondeo.Value;
								}
							}
						}
					}
					catch (Exception ex)
					{
						localPos.Observacion = $"{localDescripcion}: {ex.Message}";
						localPos.Estado = "Error";
						localPos.ColorEstado = "ROJO";
						_logger.Error(ex, localPos.Observacion);
						return;
					}


					try
					{
						//BD BCT
						var (cantidadTransacciones, montoTransacciones) = await _repositorioTrxHeader.ObtenerCantidadTransacciones(Convert.ToInt32(localPos.CodLocal), fecha);

						localPos.CanTransaccionesBCT = cantidadTransacciones;
						localPos.MontoTransaccionesBCT = montoTransacciones;

						// Estado y observacion
						var diferenciaCantidad = Math.Abs(localPos.CantTransaccionesLocal - localPos.CanTransaccionesBCT);
						var diferenciaMonto = Math.Abs(localPos.NontoTransaccionesLocal - localPos.MontoTransaccionesBCT);

						if (diferenciaCantidad == 0 && diferenciaMonto == 0)
						{
							localPos.Estado = "OK";
							localPos.Observacion = "";
							localPos.ColorEstado = "VERDE";
						}
						else if (diferenciaCantidad != 0 && diferenciaMonto != 0)
						{
							localPos.Estado = "Error";
							localPos.Observacion = $"Se halló una diferencia de {diferenciaCantidad} transacciones con un monto total de {diferenciaMonto} soles";
							localPos.ColorEstado = "ROJO";
						}
						else if (diferenciaCantidad != 0)
						{
							localPos.Estado = "Error";
							localPos.Observacion = $"Se halló una diferencia de {diferenciaCantidad} transacciones";
							localPos.ColorEstado = "AMARILLO";
						}
						else if (diferenciaMonto != 0)
						{
							localPos.Estado = "Error";
							localPos.Observacion = $"Se halló una diferencia de {diferenciaMonto} soles";
							localPos.ColorEstado = "AMARILLO";
						}
					}
					catch (Exception ex)
					{
						respuesta.Ok = false;
						respuesta.Mensaje = $"{localDescripcion}: Ocurrio un error al obtener información de la BD BCT";
						_logger.Error(ex, respuesta.Mensaje);
					}

				});

				await Task.WhenAll(tasks);
			}

			stopwatch.Stop();
			TimeSpan tiempoTranscurrido = stopwatch.Elapsed;
			string tiempoFormateado = string.Format("{0:D2}:{1:D2}:{2:D2}",
													tiempoTranscurrido.Hours,
													tiempoTranscurrido.Minutes,
													tiempoTranscurrido.Seconds);


			_logger.Information($"Fin proceso Monitor Diferencia de transacciones y montos, fecha: {fecha}, duración: {tiempoFormateado}");
			respuesta.Mensaje = $"Proceso finalizado en {tiempoFormateado}";
			return respuesta;
		}

		private async Task<bool> VerificarExistenciaProceso(ISGPContexto contexto) =>
			await contexto.RepositorioProceso.Existe(x => x.CodProceso == Constantes.CodigoProcesoDiferenciaTransacciones && x.IndActivo == "A");

		private async Task<List<Mae_Empresa>> ObtenerEmpresasActivas(ISGPContexto contexto, List<string> empresas) =>
			await contexto.RepositorioMaeEmpresa.Obtener(x => empresas.Contains(x.CodEmpresa)).ToListAsync();

		private async Task<List<ProcesarMonitorLocalBCTDTO>> ObtenerLocalesDeEmpresas(ISGPContexto contexto, List<Mae_Empresa> empresas)
		{
			var locales = new List<Mae_Local>();
			foreach (var empresa in empresas)
			{
				var existeProcesoEmpresa = await contexto.RepositorioProcesoEmpresa.Existe(x => x.CodEmpresa == empresa.CodEmpresa && x.IndActivo == "S");
				if (!existeProcesoEmpresa)
				{
					var mensaje = $"No existe proceso para la empresa o esta inactivo {empresa.Ruc}-{empresa.NomEmpresa}";
					_logger.Information(mensaje);
					continue;
				}

				locales.AddRange(await ObtenerLocales(contexto, empresa.CodEmpresa));
			}

			return locales.Select(local =>
				new ProcesarMonitorLocalBCTDTO
				{
					CodLocal = local.CodLocal,
					NomLocal = local.NomLocal,
					IP = local.Ip,
					CodEmpresa = local.CodEmpresa,
					NomEmpresa = empresas.FirstOrDefault(x => x.CodEmpresa == local.CodEmpresa)?.NomEmpresa,
				}).ToList();
		}

		private async Task<List<Mae_Local>> ObtenerLocales(ISGPContexto contexto, string codEmpresa) =>
			await contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == codEmpresa).ToListAsync();

		private string ArmarCadenaConexion(ProcesarMonitorLocalBCTDTO local)
		{
			var parametros = procesoParametroEmpresas.Where(x => x.CodEmpresa == local.CodEmpresa);
			var usuario = parametros.FirstOrDefault(x => x.CodParametro == "01")?.ValParametro;
			var clave = parametros.FirstOrDefault(x => x.CodParametro == "02")?.ValParametro;
			return $"Host={local.IP}; Database=webfront; Username={usuario}; Password={clave}";
		}


		private decimal? ExtraerValorDeParametro(string trama, string parametro)
		{
			trama = trama.Replace("S/.", ""); // Eliminamos el símbolo de la moneda
			// Buscamos la posición del parámetro en la trama.
			int posicionParametro = trama.IndexOf(parametro);
			if (posicionParametro == -1)
			{
				// Si el parámetro no se encuentra, retornamos null.
				return null;
			}

			// Calculamos la posición inicial del valor, que está justo después del parámetro.
			// Sumamos la longitud del parámetro y los espacios que puedan existir.
			int posicionInicioValor = posicionParametro + parametro.Length;
			while (posicionInicioValor < trama.Length && trama[posicionInicioValor] == ' ')
			{
				// Avanzamos hasta encontrar un carácter que no sea un espacio.
				posicionInicioValor++;
			}

			// Extraemos el substring que contiene el valor.
			// Asumimos que el valor termina al encontrar un espacio después del número o al final de la trama.
			int posicionFinValor = posicionInicioValor;
			while (posicionFinValor < trama.Length && (char.IsDigit(trama[posicionFinValor]) || trama[posicionFinValor] == '.'))
			{
				// Avanzamos hasta encontrar un carácter que no sea un dígito o punto.
				posicionFinValor++;
			}

			string valorStr = trama.Substring(posicionInicioValor, posicionFinValor - posicionInicioValor);

			// Intentamos convertir el substring extraído a decimal.
			if (decimal.TryParse(valorStr, out decimal valor))
			{
				return valor;
			}

			// Si la conversión falla, retornamos null.
			return null;
		}


		private async Task<List<ElectronicJournal>> EjecutarConReintento(Func<Task<List<ElectronicJournal>>> funcion, int reintentos = 3)
		{
			for (int intento = 0; intento < reintentos; intento++)
			{
				try
				{
					return await funcion();
				}
				catch (Exception ex) when (intento < reintentos)
				{
					_logger.Warning(ex, $"Intento Nro {intento + 1} Error al ejecutar la consulta. Intentando de nuevo...");
				}
			}
			throw new Exception("No se tienen conexión con el local");
		}

	}
}
