using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands
{
	public class ReporteDiferenciaCajaExcelCommand : IRequest<DescargarMaestroDTO>
	{
		public string CodigoEmpresa { get; set; }
		public string CodigoLocal { get; set; }
		public DateTime FechaInicio { get; set; }
		public DateTime FechaFin { get; set; }
	}

	public class ReporteDiferenciaCajaExcelHandler : IRequestHandler<ReporteDiferenciaCajaExcelCommand, DescargarMaestroDTO>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public ReporteDiferenciaCajaExcelHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}

		public async Task<DescargarMaestroDTO> Handle(ReporteDiferenciaCajaExcelCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new DescargarMaestroDTO();

			try
			{
				var dt = await _repositorioCajero.ReporteDiferenciaCajasExcel(request.CodigoEmpresa, string.IsNullOrEmpty(request.CodigoLocal) ? "0" : request.CodigoLocal, request.FechaInicio, request.FechaFin);
				dt.TableName = "Reporte";
				string fileName = $"DiferenciaCaja_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
				using (XLWorkbook wb = new XLWorkbook())
				{
					wb.Worksheets.Add(dt);
					using (MemoryStream stream = new MemoryStream())
					{
						wb.SaveAs(stream);
						respuesta.Archivo = Convert.ToBase64String(stream.ToArray());
						respuesta.NombreArchivo = fileName;
						respuesta.Ok = true;
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
