using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
	public class ObtenerParametrosQuery : IRequest<GenericResponseDTO<List<ParametrosMonitorBctDTO>>>
	{
	}

	public class ObtenerParametrosHandler : IRequestHandler<ObtenerParametrosQuery, GenericResponseDTO<List<ParametrosMonitorBctDTO>>>
	{
		private readonly ISGPContexto _contexto;

		public ObtenerParametrosHandler()
		{
			_contexto = new SGPContexto();
		}

		public async Task<GenericResponseDTO<List<ParametrosMonitorBctDTO>>> Handle(ObtenerParametrosQuery request, CancellationToken cancellationToken)
		{
			var parametrosMonitorBctDTO = new GenericResponseDTO<List<ParametrosMonitorBctDTO>> { Ok = true };
			try
			{
				var parametros = await _contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoBct).ToListAsync();

				parametrosMonitorBctDTO.Data
					= parametros.Where(x => x.CodParametro == Constantes.CodigoParametroToleranciaCantidad)
					.Select(x => new ParametrosMonitorBctDTO { ToleranciaCantidad = Convert.ToInt32(x.ValParametro), CodEmpresa = x.CodEmpresa }).ToList();
			}
			catch (Exception ex)
			{
				parametrosMonitorBctDTO.Ok = false;
				parametrosMonitorBctDTO.Mensaje = ex.Message;
			}

			return parametrosMonitorBctDTO;

		}
	}
}
