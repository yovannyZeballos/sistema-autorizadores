using MediatR;
using Microsoft.Reporting.WebForms;
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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zen.Barcode;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands
{
	public class ImprimirBarrasAutorizadorCommand : IRequest<ImprimirAutorizadorResponseDTO>
	{
		public List<ImprimirAutorizadorDTO> Autorizadores { get; set; }
		public string Usuario { get; set; }
		public string NomEmpresa { get; set; }
		public string NomLocal { get; set; }
    }

	public sealed class ImprimirBarrasAutorizadorHandler : IRequestHandler<ImprimirBarrasAutorizadorCommand, ImprimirAutorizadorResponseDTO>
	{
		private readonly ILogger _logger;
		private readonly IBCTContexto _contexto;

		public ImprimirBarrasAutorizadorHandler()
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		public async Task<ImprimirAutorizadorResponseDTO> Handle(ImprimirBarrasAutorizadorCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new ImprimirAutorizadorResponseDTO { Ok = true };

			try
			{
				foreach (var autorizador in request.Autorizadores)
				{
					autorizador.CodLocal = autorizador.CodLocal.PadLeft(4, '0');
					var existeImpresion = await _contexto.RepositorioAutImpresion.Existe(x => x.CodLocal == autorizador.CodLocal && x.CodAutorizador == autorizador.CodAutorizador);
					if (existeImpresion)
					{
						respuesta.Ok = false;
						respuesta.Mensaje += $"El autorizador {autorizador.NomAutorizador} ya fue impreso.\n";
						autorizador.Imprimir = false;
						_logger.Information(respuesta.Mensaje);
						continue;
					}

					var impresion = new AutImpresion(autorizador.CodColaborador, autorizador.CodLocal, autorizador.CodAutorizador, request.Usuario);
					_contexto.RepositorioAutImpresion.Agregar(impresion);

					_logger.Information($"Impresion insertada en la BD del autorizador con codigo {autorizador.CodAutorizador}");
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
					respuesta.Mensaje += "Utilizar el botón Reimpresión.";
				}
				else
				{
					respuesta.Mensaje = "Impresión realizada con éxito.";
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
