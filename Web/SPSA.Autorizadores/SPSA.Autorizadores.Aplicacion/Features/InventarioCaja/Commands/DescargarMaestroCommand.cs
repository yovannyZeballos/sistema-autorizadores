using ClosedXML.Excel;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
	public class DescargarMaestroCommand : IRequest<DescargarMaestroDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
	}

	public class DescargarMaestroHandler : IRequestHandler<DescargarMaestroCommand, DescargarMaestroDTO>
	{
		private readonly IRepositorioSovosInventarioCaja _repositorioSovosInventarioCaja;

		public DescargarMaestroHandler(IRepositorioSovosInventarioCaja repositorioSovosInventarioCaja)
		{
			_repositorioSovosInventarioCaja = repositorioSovosInventarioCaja;
		}

		public async Task<DescargarMaestroDTO> Handle(DescargarMaestroCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new DescargarMaestroDTO();

			try
			{
				var dt = await _repositorioSovosInventarioCaja.DescargarMaestro(request.CodEmpresa, request.CodFormato, request.CodLocal);
				dt.TableName = "Inventario";
				string fileName = $"MaestroInventarioCaja_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
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
