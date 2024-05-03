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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
	public class ProcesarActualizacionEstadoCierreCommand : IRequest<RespuestaComunDTO>
	{
		public string CodEmpresa { get; set; }
	}

	public class ProcesarActualizacionEstadoCierreHandler : IRequestHandler<ProcesarActualizacionEstadoCierreCommand, RespuestaComunDTO>
	{
		private IBCTContexto _contexto;
		private readonly ILogger _logger;
		private readonly IRepositorioMonitorProcesoCierre _repositorioMonitorProcesoCierre;


		public ProcesarActualizacionEstadoCierreHandler(IRepositorioMonitorProcesoCierre repositorioMonitorProcesoCierre)
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
			_repositorioMonitorProcesoCierre = repositorioMonitorProcesoCierre;
		}

		public async Task<RespuestaComunDTO> Handle(ProcesarActualizacionEstadoCierreCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO { Ok = true };
			var fechaFin = DateTime.Now;
			var fechaInicio = fechaFin.AddDays(-40);
			const int batchSize = 1000; // Define el tamaño del lote aquí

			try
			{
				var localesAlternos = await _contexto.RepositorioMaeLocalAlterno.Obtener().ToListAsync();
				var empresas = string.IsNullOrEmpty(request.CodEmpresa)
					? await _contexto.RepositorioProcesoEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoActualizacionEstadoCierre).ToListAsync()
					: new List<ProcesoEmpresa> { new ProcesoEmpresa { CodEmpresa = request.CodEmpresa } };

				//Elimina data de la tabla temporal
				await ELiminarDataTablaTemporal();

				foreach (var empresa in empresas)
				{
					try
					{
						var parametros = await _contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoActualizacionEstadoCierre &&
																								 x.CodEmpresa == empresa.CodEmpresa).ToListAsync();

						var formatoCodigoAlterno = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroFormatoCodigoAlterno_ProcesoCierre).Select(x => Convert.ToInt32(x.ValParametro)).FirstOrDefault();

						var cadenaConexion = ArmarCadenaconexion(parametros);
						var dataCT2 = await _repositorioMonitorProcesoCierre.ObtenerDatos(fechaInicio, fechaFin, cadenaConexion);

						_logger.Information($"Se obtuvieron {dataCT2.Count} registros de la empresa {empresa.CodEmpresa} en este rango de fechas {fechaInicio} - {fechaFin}");

						var monCierreLocales = dataCT2.Select(cierre =>
						{
							var codigoAlterno = cierre.CodLocal + formatoCodigoAlterno;

							var maeLocalAlterno = localesAlternos.FirstOrDefault(x => x.CodLocalAlterno == codigoAlterno);
							if (maeLocalAlterno == null)
							{
								_logger.Error($"No se encontro el local alterno {codigoAlterno} en la empresa {empresa.CodEmpresa}");
								return null;
							}

							

							return new TmpMonCierreLocal
							{
								CodLocalAlterno = codigoAlterno,
								FechaCierre = cierre.FechaCierre,
								FechaContable = cierre.FechaCierreContable,
								TipEstado = cierre.Estado,
								FechaCarga = DateTime.Now,
								HoraCarga = DateTime.Now.TimeOfDay
							};
						}).Where(x => x != null).ToList();


						

						// Inserta los registros en lotes en tabla temporal
						for (int i = 0; i < monCierreLocales.Count; i += batchSize)
						{
							var batch = monCierreLocales.Skip(i).Take(batchSize).ToList();
							await _contexto.RepositorioTmpMonCierreLocal.BulkInsert(batch);
						}

						// Actualiza los locales con la información de la tabla temporal
						

					}
					catch (Exception ex)
					{
						_logger.Error(ex, $"Ocurrio un error al obtener los registros para la empresa {empresa.CodEmpresa} en este rango de fechas {fechaInicio} - {fechaFin} | {respuesta.Mensaje}");
					}
				}

				await _contexto.RepositorioMonCierreLocal.InsertarActualizar();
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
				_logger.Error(ex, respuesta.Mensaje);
			}

			return respuesta;
		}

		private string ArmarCadenaconexion(List<ProcesoParametroEmpresa> parametros)
		{
			var servidor = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroServidorBD_ProcesoCierre).Select(x => x.ValParametro).FirstOrDefault();
			var serviceName = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroServiceNameBD_ProcesoCierre).Select(x => x.ValParametro).FirstOrDefault();
			var puerto = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroPuertoBD_ProcesoCierre).Select(x => Convert.ToInt32(x.ValParametro)).FirstOrDefault();
			var usuario = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroUsuarioBD_ProcesoCierre).Select(x => x.ValParametro).FirstOrDefault();
			var clave = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroClaveBD_ProcesoCierre).Select(x => x.ValParametro).FirstOrDefault();

			if (string.IsNullOrEmpty(servidor) || string.IsNullOrEmpty(serviceName) || puerto == 0 ||
				string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
				return null;

			return $"Data Source = (DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {servidor})(PORT = {puerto})) (CONNECT_DATA =(SERVER = DEDICATED)" +
				$"(SERVICE_NAME = {serviceName}))); User Id = {usuario}; Password = {clave}; Min Pool Size=1; Max Pool Size=10;";
		}

		private async Task ELiminarDataTablaTemporal()
		{
			await _contexto.RepositorioTmpMonCierreLocal.Truncate();
		}

		private void ActualizarEstadoCierre(List<MonCierreLocal> localesActualizar, List<MonCierreLocal> locales)
		{
			var localesParaActualizar = from localActualizar in localesActualizar
										join local in locales
										on new { localActualizar.CodLocalAlterno, localActualizar.FechaContable } equals new { local.CodLocalAlterno, local.FechaContable }
										select new { localActualizar, local };

			foreach (var local in localesParaActualizar)
			{
				local.localActualizar.FechaCierre = local.local.FechaCierre;
				local.localActualizar.TipEstado = local.local.TipEstado;
				local.localActualizar.FechaCarga = DateTime.Now;
				local.localActualizar.HoraCarga = DateTime.Now.TimeOfDay;
			}
		}
	}
}
