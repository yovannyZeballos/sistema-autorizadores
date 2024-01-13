using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
	public class ObtenerParametrosQuery : IRequest<ParametrosMonitorBctDTO>
	{
	}

	public class ObtenerParametrosHandler : IRequestHandler<ObtenerParametrosQuery, ParametrosMonitorBctDTO>
	{
		private readonly IRepositorioProcesoParametro _repositorioProcesoParametro;

		public ObtenerParametrosHandler(IRepositorioProcesoParametro repositorioProcesoParametro)
		{
			_repositorioProcesoParametro = repositorioProcesoParametro;
		}

		public async Task<ParametrosMonitorBctDTO> Handle(ObtenerParametrosQuery request, CancellationToken cancellationToken)
		{
			var parametrosMonitorBctDTO = new ParametrosMonitorBctDTO { Ok = true };
			try
			{
				var parametros = await _repositorioProcesoParametro.ListarPorProceso(Constantes.CodigoProcesoBct);
				parametrosMonitorBctDTO.ToleranciaSegundos 
					= Convert.ToInt32(parametros.FirstOrDefault(x => x.CodParametro == Constantes.CodigoParametroToleranciaSegundos)?.ValParametro);
				parametrosMonitorBctDTO.ToleranciaCantidad
					= Convert.ToInt32(parametros.FirstOrDefault(x => x.CodParametro == Constantes.CodigoParametroToleranciaCantidad)?.ValParametro);

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
