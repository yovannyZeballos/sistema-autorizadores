using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Entities;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Aplicacion.ReportData;
using SPSA.Autorizadores.Aplicacion.ReportUtils;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands
{
	public class ReimprimirBarrasAutorizadorCommand : IRequest<ImprimirAutorizadorResponseDTO>
	{
		public List<ImprimirAutorizadorDTO> Autorizadores { get; set; }
		public string Motivo { get; set; }
		public string NomEmpresa { get; set; }
		public string NomLocal { get; set; }
		public string Usuario { get; set; }
	}

	public sealed class ReimprimirBarrasAutorizadorHandler : IRequestHandler<ReimprimirBarrasAutorizadorCommand, ImprimirAutorizadorResponseDTO>
	{
		private readonly ILogger _logger;
		private readonly ISGPContexto _contexto;

		public ReimprimirBarrasAutorizadorHandler()
		{
			_contexto = new SGPContexto();
			_logger = SerilogClass._log;
		}

		public async Task<ImprimirAutorizadorResponseDTO> Handle(ReimprimirBarrasAutorizadorCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new ImprimirAutorizadorResponseDTO { Ok = true };

			try
			{
				foreach (var autorizador in request.Autorizadores)
				{
					//autorizador.CodLocal = autorizador.CodLocal.PadLeft(4, '0');

					var autImpresionBd = await _contexto.RepositorioAutImpresion.Obtener(x => x.CodLocal == autorizador.CodLocal && x.CodAutorizador == autorizador.CodAutorizador).AsNoTracking().SingleOrDefaultAsync();


					if (autImpresionBd == null)
					{
						respuesta.Ok = false;
						respuesta.Mensaje += $"El autorizador {autorizador.NomAutorizador} no ha sido impreso.\n";
						autorizador.Imprimir = false;
						_logger.Information(respuesta.Mensaje);
						continue;
					}

					var existeImpresion = await _contexto.RepositorioAutImpresion.Existe(x => x.CodLocal == autorizador.CodLocal && x.CodAutorizador == autorizador.CodAutorizador);

					var impresion = AutImpresion.Actualizar(autImpresionBd.Id, autImpresionBd.CodColaborador, autImpresionBd.CodLocal, autImpresionBd.CodAutorizador, autImpresionBd.UsuImpresion, autImpresionBd.FecImpresion, autImpresionBd.Correlativo, request.Motivo, request.Usuario);

					_contexto.RepositorioAutImpresion.Actualizar(impresion);

					_logger.Information($"Impresion actualizada en la BD del autorizador con codigo {autorizador.CodAutorizador}");
				}
				
				await _contexto.GuardarCambiosAsync();

				List<ReportDataAutorizadores> reportData = request.Autorizadores
					.Where(x => x.Imprimir)
					.Select(x => new ReportDataAutorizadores
					{
						Autorizador = x.NomAutorizador,
						Cargo = x.Cargo,
						Empresa = request.NomEmpresa,
						Local = request.NomLocal,
						CodigoBarras = ReportePDFServicio.GenerarCodigoDeBarras(x.CodAutorizador)
					}).ToList();

				if (!respuesta.Ok)
				{
					respuesta.Mensaje += "Utilizar el botón Impresión.";
				}
				else
				{
					respuesta.Mensaje = "ReImpresión realizada con éxito.";
				}

				if (reportData.Count == 0)
				{
					return respuesta;
				}

				var parametros = new Dictionary<string, string>
				{
					{ "Fecha", DateTime.Now.ToString() }
				};

				respuesta.Contenido = ReportePDFServicio.GenerarReportePDFBase64(reportData, "Autorizadores", parametros, @"Reportes\rptBarraAutorizador.rdl");
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
				_logger.Error(ex, respuesta.Mensaje);
			}

			return respuesta;
		}
	}
}
