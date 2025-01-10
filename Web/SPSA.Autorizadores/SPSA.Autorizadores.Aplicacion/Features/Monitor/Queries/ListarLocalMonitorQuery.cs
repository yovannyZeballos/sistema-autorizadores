using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
	public class ListarLocalMonitorQuery : IRequest<ListarComunDTO<Dictionary<string, object>>>
	{
		public string CodEmpresa { get; set; }
		public string Fecha { get; set; }
		public string Estado { get; set; }
		public int Tipo { get; set; }
	}

	public class ListarLocalMonitorHandler : IRequestHandler<ListarLocalMonitorQuery, ListarComunDTO<Dictionary<string, object>>>
	{
		private readonly IRepositorioMonitorReporte _repositorioLocalMonitor;

		public ListarLocalMonitorHandler(IRepositorioMonitorReporte repositorioLocalMonitor)
		{
			_repositorioLocalMonitor = repositorioLocalMonitor;
		}

		public async Task<ListarComunDTO<Dictionary<string, object>>> Handle(ListarLocalMonitorQuery request, CancellationToken cancellationToken)
		{
			var response = new ListarComunDTO<Dictionary<string, object>> { Ok = true };

			try
			{
				var fechaValida = DateTime.TryParseExact(request.Fecha, "dd/MM/yyyy", new CultureInfo("es-PE"), DateTimeStyles.None, out DateTime fecha);

				if (!fechaValida)
				{
					response.Ok = false;
					response.Mensaje = "El formato de la fecha ingresada es invalida";
					return response;
				}


				response.Columnas = new List<string>();
				response.Data = new List<Dictionary<string, object>>();

				using (ISGPContexto contexto = new SGPContexto())
				{
					var lista = await contexto.RepositorioMonCierreEOD.Obtener(x => (request.CodEmpresa == "0" || x.CodEmpresa == request.CodEmpresa) &&
																					x.FechaCierre == fecha &&
																					(request.Estado == "0" || x.Estado == request.Estado) &&
																					x.Tipo == request.Tipo)
						.Join(contexto.RepositorioMaeLocal.Obtener(),
							  cierre => new { cierre.CodEmpresa, cierre.CodCadena, cierre.CodZona, cierre.CodRegion, cierre.CodLocal },
							  local => new { local.CodEmpresa, local.CodCadena, local.CodZona, local.CodRegion, local.CodLocal },
							  (cierre, local) => new { Cierre = cierre, Local = local })
						.Join(contexto.RepositorioMaeEmpresa.Obtener(),
							  cierreLocal => cierreLocal.Cierre.CodEmpresa,
							  empresa => empresa.CodEmpresa,
							  (cierreLocal, empresa) => new { CierreLocal = cierreLocal, Empresa = empresa })
						.Where(x => x.CierreLocal.Local.TipEstado == "A")
						.ToListAsync();

					if (request.Tipo == 1)
					{
						response.Columnas.AddRange(new List<string> { "TIP_ESTADO", "NOM EMPRESA", "COD", "NOM LOCAL", "IP", "F.CIERRE", "ESTADO", "INICIO", "FIN", "OBS", "FEC PROCESO" });

						response.Data = lista.Select(item => new Dictionary<string, object>
						{
							{ "TIP_ESTADO", item.CierreLocal.Cierre.Estado },
							{ "NOMEMPRESA", item.Empresa.NomEmpresa },
							{ "COD", item.CierreLocal.Cierre.CodLocal },
							{ "NOMLOCAL", item.CierreLocal.Local.NomLocal },
							{ "IP", item.CierreLocal.Local.Ip },
							{ "FCIERRE", item.CierreLocal.Cierre.FechaCierre?.ToString("dd/MM/yyyy") },
							{ "ESTADO", GetEstadoCierreEOD(item.CierreLocal.Cierre.Estado) },
							{ "INICIO", item.CierreLocal.Cierre.HoraInicio },
							{ "FIN", item.CierreLocal.Cierre.HoraFin },
							{ "OBS", item.CierreLocal.Cierre.Observacion },
							{ "FECPROCESO", item.CierreLocal.Cierre.FechaProceso.ToString("dd/MM/yyyy HH:mm:ss") }
						}).ToList();
					}
					else
					{
						response.Columnas.AddRange(new List<string> { "TIP_ESTADO", "EMPRESA", "COD", "NOM LOCAL", "IP SERVER", "¿CAJA DEFECTUOSA?", "OBS", "FEC PROCESO" });
						response.Data = lista.Select(item => new Dictionary<string, object>
						{
							{ "TIP_ESTADO", item.CierreLocal.Cierre.Estado },
							{ "EMPRESA", item.Empresa.NomEmpresa },
							{ "COD", item.CierreLocal.Cierre.CodLocal },
							{ "NOMLOCAL", item.CierreLocal.Local.NomLocal },
							{ "IPSERVER", item.CierreLocal.Local.Ip },
							{ "¿CAJADEFECTUOSA?", GetEstadoCajaDefectuosa(item.CierreLocal.Cierre.Estado) },
							{ "OBS", item.CierreLocal.Cierre.Observacion },
							{ "FECPROCESO", item.CierreLocal.Cierre.FechaProceso.ToString("dd/MM/yyyy HH:mm:ss") }
						}).ToList();
					}

					if (response.Data.Count == 0)
					{
						response.Ok = false;
						response.Mensaje = "No se encuentra información de cierre sobre la fecha ingresada.";
						return response;
					}
				}
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}
			return response;
		}

		private string GetEstadoCierreEOD(string estado)
		{
			switch (estado)
			{
				case "1":
					return "CIERRE REALIZADO";
				case "2":
					return "PENDIENTE VALIDACION DE CIERRE";
				case "3":
					return "NO SE HA REALIZADO CIERRE";
				default:
					return string.Empty;
			}
		}

		private string GetEstadoCajaDefectuosa(string estado)
		{
			switch (estado)
			{
				case "2":
					return "PENDIENTE VALIDACION DE CIERRE";
				case "4":
					return "SI";
				case "5":
					return "NO";
				default:
					return string.Empty;
			}
		}
	}

}
