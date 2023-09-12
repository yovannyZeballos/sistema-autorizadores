using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands
{
	public class DescargarExcelRolesCommand : IRequest<DescargarMaestroDTO>
	{
		public string CodEmpresa { get; set; }
	}

	public class DescargarExcelRolesHandler : IRequestHandler<DescargarExcelRolesCommand, DescargarMaestroDTO>
	{
		private readonly IRepositorioPuesto _repositorioPuesto;

		public DescargarExcelRolesHandler(IRepositorioPuesto repositorioPuesto)
		{
			_repositorioPuesto = repositorioPuesto;
		}

		public async Task<DescargarMaestroDTO> Handle(DescargarExcelRolesCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new DescargarMaestroDTO();

			try
			{
				var dt = await _repositorioPuesto.Listar(request.CodEmpresa);
				dt.TableName = "Roles";
				string fileName = $"MaestroRoles_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
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
