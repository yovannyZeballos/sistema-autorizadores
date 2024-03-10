using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Login.Queries
{
	public class ObtenerJerarquiaOrganizacionalQuery : IRequest<JerarquiaOrganizacionalDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodCadena { get; set; }
		public string CodRegion { get; set; }
		public string CodZona { get; set; }
		public string CodLocal { get; set; }
	}

	public class ObtenerJerarquiaOrganizacionalHandler : IRequestHandler<ObtenerJerarquiaOrganizacionalQuery, JerarquiaOrganizacionalDTO>
	{
		private readonly IBCTContexto _bCTContexto;
		private readonly ILogger _logger;

		public ObtenerJerarquiaOrganizacionalHandler()
		{
			_bCTContexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		public async Task<JerarquiaOrganizacionalDTO> Handle(ObtenerJerarquiaOrganizacionalQuery request, CancellationToken cancellationToken)
		{
			var response = new JerarquiaOrganizacionalDTO { Ok = true };

			try
			{
				var local = await _bCTContexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa &&
																				x.CodCadena == request.CodCadena &&
																				x.CodRegion == request.CodRegion &&
																				x.CodZona == request.CodZona &&
																				x.CodLocal == request.CodLocal).FirstOrDefaultAsync();

				var zona = await _bCTContexto.RepositorioMaeZona.Obtener(x => x.CodEmpresa == request.CodEmpresa &&
																			  x.CodCadena == request.CodCadena &&
																			  x.CodRegion == request.CodRegion &&
																			  x.CodZona == request.CodZona).FirstOrDefaultAsync();

				var region = await _bCTContexto.RepositorioMaeRegion.Obtener(x => x.CodEmpresa == request.CodEmpresa &&
																				  x.CodCadena == request.CodCadena &&
																				  x.CodRegion == request.CodRegion).FirstOrDefaultAsync();

				var cadena = await _bCTContexto.RepositorioMaeCadena.Obtener(x => x.CodEmpresa == request.CodEmpresa &&
																				  x.CodCadena == request.CodCadena).FirstOrDefaultAsync();

				var empresa = await _bCTContexto.RepositorioMaeEmpresa.Obtener(x => x.CodEmpresa == request.CodEmpresa).FirstOrDefaultAsync();

				response.CodEmpresa = empresa.CodEmpresa;
				response.NomEmpresa = empresa.NomEmpresa;
				response.CodCadena = cadena.CodCadena;
				response.NomCadena = cadena.NomCadena;
				response.CodRegion = region.CodRegion;
				response.NomRegion = region.NomRegion;
				response.CodZona = zona.CodZona;
				response.NomZona = zona.NomZona;
				response.CodLocal = local.CodLocal;
				response.NomLocal = local.NomLocal;
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al obtener las jerarquias";
				_logger.Error(ex, response.Mensaje);
			}


			return response;
		}
	}
}
