using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Dto;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Operaciones.Queries
{
    public class ListarDocumentosElectronicosQuery : ListarDocumentoElectronicoDto, IRequest<GenericResponseDTO<PagedResult<DocumentoElectronico>>>
	{
		public string Busqueda { get; set; }
	}

	public class ListarDocumentosElectronicosHandler : IRequestHandler<ListarDocumentosElectronicosQuery, GenericResponseDTO<PagedResult<DocumentoElectronico>>>
	{
		private readonly IRepositorioTrxHeader _repositorioTrxHeader;
		private readonly ILogger _logger;


		public ListarDocumentosElectronicosHandler(IRepositorioTrxHeader repositorioTrxHeader)
		{
			_repositorioTrxHeader = repositorioTrxHeader;
			_logger = SerilogClass._log;

		}
		public async Task<GenericResponseDTO<PagedResult<DocumentoElectronico>>> Handle(ListarDocumentosElectronicosQuery request, CancellationToken cancellationToken)
		{
			var respuesta = new GenericResponseDTO<PagedResult<DocumentoElectronico>> { Ok = true };
			try
			{
				var documentos = await _repositorioTrxHeader.ListarDocumentosElectronicos(request);
			    int totalRegistros = documentos.Any() ? documentos[0].TotalRegistros : 0;
			

				respuesta.Data = new PagedResult<DocumentoElectronico>
				{
					Items = documentos.Where(x => x.ToString().ToUpper().Contains((request.Busqueda ?? "").ToUpper())).ToList(),
					PageNumber = request.NumeroPagina,
					PageSize = request.TamañoPagina,
					TotalPages = totalRegistros,
					TotalRecords = totalRegistros
				};
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
				_logger.Error(ex, respuesta.Mensaje);

			}
			return respuesta;
		}

		private static async Task<List<DocumentoElectronico>> ObtenerDatosPrueba()
		{
			await Task.Delay(1);
			return new List<DocumentoElectronico>
			{
				new DocumentoElectronico { Fecha = "01/01/2025", Caja = "CAJA01", Importe = 125.50m, DocElectronico = "BB13-7083232", MedioPago = "EFECTIVO", Cajero = "JPEREZ", TipoDocumento = "DNI", NroDocumento = "7083232", Local = "001", TipoDocElectronico = "BLT" },
				new DocumentoElectronico { Fecha = "01/01/2025", Caja = "CAJA02", Importe = 250.75m, DocElectronico = "F001-000001", MedioPago = "TARJETA_CREDITO", Cajero = "MGARCIA", TipoDocumento = "DNI", NroDocumento = "44567651", Local = "002", TipoDocElectronico = "TFC" },
			};
		}

		private static async Task<List<DocumentoElectronico>> ObtenerDatosPruebaPaginado(int numeroPagina, int tamañoPagina)
		{
			var todosLosDocumentos = await ObtenerDatosPrueba();

			// Calcular la paginación
			var skip = (numeroPagina - 1) * tamañoPagina;
			return todosLosDocumentos.Skip(skip).Take(tamañoPagina).ToList();
		}

		private static async Task<int> ObtenerTotalRegistrosPrueba()
		{
			var todosLosDocumentos = await ObtenerDatosPrueba();
			return todosLosDocumentos.Count;
		}
	}
}
