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
	/// Query para listar los locales.
	/// </summary>
	public class ListarLocalesQuery : IRequest<GenericResponseDTO<List<ListarLocalDTO>>>
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

		/// <summary>
		/// Código de zona.
		/// </summary>
		public string CodZona { get; set; }
	}

	/// <summary>
	/// Manejador para el query ListarCadenasQuery.
	/// </summary>
	public class ListarLocalesHandler : IRequestHandler<ListarLocalesQuery, GenericResponseDTO<List<ListarLocalDTO>>>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para el manejador ListarCadenasHandler.
		/// </summary>
		public ListarLocalesHandler(IMapper mapper)
		{
			_contexto = new BCTContexto();
			_mapper = mapper;
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el query ListarCadenasQuery.
		/// </summary>
		public async Task<GenericResponseDTO<List<ListarLocalDTO>>> Handle(ListarLocalesQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarLocalDTO>> { Ok = true, Data = new List<ListarLocalDTO>() };

			try
			{
				var locales = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa 
																				&& x.CodCadena == request.CodCadena
																				&& x.CodRegion == request.CodRegion
																				&& x.CodZona == request.CodZona).ToListAsync();
				var localesAsociadas = await _contexto.RepositorioSegLocal.Obtener(x => x.CodUsuario == request.CodUsuario 
																					  && x.CodEmpresa == request.CodEmpresa
																					  && x.CodCadena == request.CodCadena
																					  && x.CodRegion == request.CodRegion
																					  && x.CodZona == request.CodZona).ToListAsync();

				var localesDto = _mapper.Map<List<ListarLocalDTO>>(locales);

				foreach (var item in localesDto)
				{
					item.IndAsociado = localesAsociadas.Exists(x => x.CodLocal == item.CodLocal);
				}

				response.Data = localesDto.OrderBy(x => x.CodZona).ToList();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar los locales";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
