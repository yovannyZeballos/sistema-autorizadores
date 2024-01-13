using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
	public class ObtenerRegistrosMonitorBCTQuery : IRequest<ObtenerComunDTO<(TransactionXmlCT2, List<TransactionXmlCT2>)>>
	{
	}

	public class ObtenerRegistrosMonitorBCTHandler : IRequestHandler<ObtenerRegistrosMonitorBCTQuery, ObtenerComunDTO<(TransactionXmlCT2, List<TransactionXmlCT2>)>>
	{
		private readonly IRepositorioTransactionXmlCT2 _repositorioTransactionXmlCT2;

		public ObtenerRegistrosMonitorBCTHandler(IRepositorioTransactionXmlCT2 repositorioTransactionXmlCT2)
		{
			_repositorioTransactionXmlCT2 = repositorioTransactionXmlCT2;
		}

		public async Task<ObtenerComunDTO<(TransactionXmlCT2, List<TransactionXmlCT2>)>> Handle(ObtenerRegistrosMonitorBCTQuery request, CancellationToken cancellationToken)
		{
			var response = new ObtenerComunDTO<(TransactionXmlCT2, List<TransactionXmlCT2>)> { Ok = true };

			try
			{
				response.Data = await _repositorioTransactionXmlCT2.Obtener();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}

			return response;
		}
	}
}
