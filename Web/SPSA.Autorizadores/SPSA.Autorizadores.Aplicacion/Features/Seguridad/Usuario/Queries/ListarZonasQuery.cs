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
	/// Query para listar las zonas.
	/// </summary>
	public class ListarZonasQuery : IRequest<GenericResponseDTO<List<ListarZonaDTO>>>
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
		/// Código de zona.
		/// </summary>
		public string CodCadena { get; set; }

		/// <summary>
		/// Código de zona.
		/// </summary>
		public string CodRegion { get; set; }
	}

	/// <summary>
	/// Manejador para el query ListarCadenasQuery.
	/// </summary>
	public class ListarZonasHandler : IRequestHandler<ListarZonasQuery, GenericResponseDTO<List<ListarZonaDTO>>>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para el manejador ListarCadenasHandler.
		/// </summary>
		public ListarZonasHandler(IMapper mapper)
		{
			_contexto = new BCTContexto();
			_mapper = mapper;
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el query ListarCadenasQuery.
		/// </summary>
		public async Task<GenericResponseDTO<List<ListarZonaDTO>>> Handle(ListarZonasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarZonaDTO>> { Ok = true, Data = new List<ListarZonaDTO>() };

			try
			{
				var zonas = await _contexto.RepositorioMaeZona.Obtener(x => x.CodEmpresa == request.CodEmpresa 
																				&& x.CodCadena == request.CodCadena
																				&& x.CodRegion == request.CodRegion).ToListAsync();
				var zonasAsociadas = await _contexto.RepositorioSegZona.Obtener(x => x.CodUsuario == request.CodUsuario 
																					  && x.CodEmpresa == request.CodEmpresa
																					  && x.CodCadena == request.CodCadena
																					  && x.CodRegion == request.CodRegion).ToListAsync();

				var zonasDto = _mapper.Map<List<ListarZonaDTO>>(zonas);

				foreach (var item in zonasDto)
				{
					item.IndAsociado = zonasAsociadas.Exists(x => x.CodZona == item.CodZona);
				}

				response.Data = zonasDto.OrderBy(x => x.CodZona).ToList();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las zonas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
