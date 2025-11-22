using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Dto;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using Stimulsoft.Base.Gauge.GaugeGeoms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Operaciones.Queries
{
    public class ListarDocumentosElectronicosQuery : ListarDocumentoElectronicoDto, IRequest<GenericResponseDTO<PagedResult<DocumentoElectronico>>>
	{
		public string Busqueda { get; set; }
		public string CodEmpresaSesion { get; set; }
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
				string empresasOracle = Convert.ToString(ConfigurationManager.AppSettings["EmpresasDocumentosElectronicosOracle"] ?? "");
				List<string> listaEmpresasOracle = empresasOracle.Split(',').ToList();

				List<DocumentoElectronico> documentos;

				if (listaEmpresasOracle.Contains(request.CodEmpresaSesion))
				{
					documentos = await _repositorioTrxHeader.ListarDocumentosElectronicosCT3(request);
				}
				else
				{
					documentos = await _repositorioTrxHeader.ListarDocumentosElectronicosSGP(request);
				}

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
	}
}
