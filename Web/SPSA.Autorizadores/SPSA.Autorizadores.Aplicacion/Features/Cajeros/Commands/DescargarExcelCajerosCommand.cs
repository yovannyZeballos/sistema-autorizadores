using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cajeros.Commands
{
	public class DescargarExcelCajerosCommand : IRequest<DescargarMaestroDTO>
	{
		public string CodigoLocal { get; set; }
	}

	public class DescargarExcelCajerosHandler : IRequestHandler<DescargarExcelCajerosCommand, DescargarMaestroDTO>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public DescargarExcelCajerosHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}

		public async Task<DescargarMaestroDTO> Handle(DescargarExcelCajerosCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new DescargarMaestroDTO();

			try
			{
				var dt = await _repositorioCajero.ListarCajero(request.CodigoLocal);
				dt.TableName = "Cajeros";
				string fileName = $"MaestroCajeros_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
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
