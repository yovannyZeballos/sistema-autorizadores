using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Linq;
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
		public string Usuario { get; set; }
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
			var response = new JerarquiaOrganizacionalDTO() ;

			try
			{
				response = await _bCTContexto.RepositorioMaeLocal
					.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena
								&& x.CodRegion == request.CodRegion && x.CodZona == request.CodZona
								&& x.CodLocal == request.CodLocal)
					.Join(_bCTContexto.RepositorioMaeZona.Obtener(),
						local => new { local.CodEmpresa, local.CodCadena, local.CodRegion, local.CodZona }, // clave de unión en Local
						zona => new { zona.CodEmpresa, zona.CodCadena, zona.CodRegion, zona.CodZona }, // clave de unión en Zona
						(local, zona) => new { Local = local, Zona = zona }) // resultado
					.Join(_bCTContexto.RepositorioMaeRegion.Obtener(),
						zona => new { zona.Zona.CodEmpresa, zona.Zona.CodCadena, zona.Zona.CodRegion }, // clave de unión en Zona
						region => new { region.CodEmpresa, region.CodCadena, region.CodRegion }, // clave de unión en Region
						(zona, region) => new { zona.Local, zona.Zona, Region = region }) // resultado
					.Join(_bCTContexto.RepositorioMaeCadena.Obtener(),
						region => new { region.Region.CodEmpresa, region.Region.CodCadena }, // clave de unión en Region
						cadena => new { cadena.CodEmpresa, cadena.CodCadena }, // clave de unión en Cadena
						(region, cadena) => new { region.Local, region.Zona, region.Region, Cadena = cadena }) // resultado
					.Join(_bCTContexto.RepositorioMaeEmpresa.Obtener(),
						cadena => cadena.Cadena.CodEmpresa, // clave de unión en Cadena
						empresa => empresa.CodEmpresa, // clave de unión en Empresa
						(cadena, empresa) => new { cadena.Local, cadena.Zona, cadena.Region, cadena.Cadena, Empresa = empresa }) // resultado
					.Select(x => new JerarquiaOrganizacionalDTO
					{
						CodEmpresa = x.Empresa.CodEmpresa,
						NomEmpresa = x.Empresa.NomEmpresa,
						CodCadena = x.Cadena.CodCadena,
						NomCadena = x.Cadena.NomCadena,
						CodRegion = x.Region.CodRegion,
						NomRegion = x.Region.NomRegion,
						CodZona = x.Zona.CodZona,
						NomZona = x.Zona.NomZona,
						CodLocal = x.Local.CodLocal,
						NomLocal = x.Local.NomLocal,
						Ok = true
					})
					.FirstOrDefaultAsync();

				response.EmpresasAsociadas = await _bCTContexto.RepositorioSegEmpresa
					.Obtener(x => x.CodUsuario == request.Usuario)
					.Include(x => x.Mae_Empresa)
					.Select(x => new EmpresaAsociadaDTO
					{
						CodEmpresa = x.CodEmpresa,
						NomEmpresa = x.Mae_Empresa.NomEmpresa
					})
					.ToListAsync();

				response.CadenasAsociadas = await _bCTContexto.RepositorioSegCadena
					.Obtener(x => x.CodUsuario == request.Usuario)
					.Include(x => x.Mae_Cadena)
					.Select(x => new CadenaAsociadaDTO
					{
						CodEmpresa = x.CodEmpresa,
						CodCadena = x.CodCadena,
						NomCadena = x.Mae_Cadena.NomCadena
					})
					.ToListAsync();

				response.RegionesAsociadas = await _bCTContexto.RepositorioSegRegion
					.Obtener(x => x.CodUsuario == request.Usuario)
					.Include(x => x.Mae_Region)
					.Select(x => new RegionAsociadaDTO
					{
						CodEmpresa = x.CodEmpresa,
						CodCadena = x.CodCadena,
						CodRegion = x.CodRegion,
						NomRegion = x.Mae_Region.NomRegion
					})
					.ToListAsync();

				response.ZonasAsociadas = await _bCTContexto.RepositorioSegZona
					.Obtener(x => x.CodUsuario == request.Usuario)
					.Include(x => x.Mae_Zona)
					.Select(x => new ZonaAsociadaDTO
					{
						CodEmpresa = x.CodEmpresa,
						CodCadena = x.CodCadena,
						CodRegion = x.CodRegion,
						CodZona = x.CodZona,
						NomZona = x.Mae_Zona.NomZona
					})
					.ToListAsync();

				response.LocalesAsociados = await _bCTContexto.RepositorioSegLocal
					.Obtener(x => x.CodUsuario == request.Usuario)
					.Include(x => x.Mae_Local)
					.Select(x => new LocalAsociadoDTO
					{
						CodEmpresa = x.CodEmpresa,
						CodCadena = x.CodCadena,
						CodRegion = x.CodRegion,
						CodZona = x.CodZona,
						CodLocal = x.CodLocal,
						NomLocal = x.Mae_Local.NomLocal
					})
					.ToListAsync();
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
