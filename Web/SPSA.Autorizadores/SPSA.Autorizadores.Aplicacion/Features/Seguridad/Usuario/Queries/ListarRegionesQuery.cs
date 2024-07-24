using AutoMapper;
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
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Queries
{
	/// <summary>
	/// Query para listar las cadenas.
	/// </summary>
	public class ListarRegionesQuery : IRequest<GenericResponseDTO<List<ListarRegionDTO>>>
	{
		/// <summary>
		/// Código de usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Código de empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Código de cadena.
		/// </summary>
		public string CodCadena { get; set; }
	}

	/// <summary>
	/// Manejador para el query ListarCadenasQuery.
	/// </summary>
	public class ListarRegionesHandler : IRequestHandler<ListarRegionesQuery, GenericResponseDTO<List<ListarRegionDTO>>>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para el manejador ListarCadenasHandler.
		/// </summary>
		public ListarRegionesHandler(IMapper mapper)
		{
			_contexto = new SGPContexto();
			_mapper = mapper;
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el query ListarCadenasQuery.
		/// </summary>
		public async Task<GenericResponseDTO<List<ListarRegionDTO>>> Handle(ListarRegionesQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarRegionDTO>> { Ok = true, Data = new List<ListarRegionDTO>() };

			try
			{
				var regiones = await _contexto.RepositorioMaeRegion.Obtener(x => x.CodEmpresa == request.CodEmpresa 
																				&& x.CodCadena == request.CodCadena).ToListAsync();
				var regionesAsociadas = await _contexto.RepositorioSegRegion.Obtener(x => x.CodUsuario == request.CodUsuario 
																					  && x.CodEmpresa == request.CodEmpresa
																					  && x.CodCadena == request.CodCadena).ToListAsync();

				var regionesDto = _mapper.Map<List<ListarRegionDTO>>(regiones);

				foreach (var item in regionesDto)
				{
					item.IndAsociado = regionesAsociadas.Exists(x => x.CodRegion == item.CodRegion);
				}

				response.Data = regionesDto.OrderBy(x => x.CodRegion).ToList();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las regiones";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
