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
	public class ListarCadenasQuery : IRequest<GenericResponseDTO<List<ListarCadenaDTO>>>
	{
		/// <summary>
		/// Código de usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Código de empresa.
		/// </summary>
		public string CodEmpresa { get; set; }
	}

	/// <summary>
	/// Manejador para el query ListarCadenasQuery.
	/// </summary>
	public class ListarCadenasHandler : IRequestHandler<ListarCadenasQuery, GenericResponseDTO<List<ListarCadenaDTO>>>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;


		/// <summary>
		/// Constructor para el manejador ListarCadenasHandler.
		/// </summary>
		public ListarCadenasHandler(IMapper mapper)
		{
			_contexto = new SGPContexto();
			_mapper = mapper;
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el query ListarCadenasQuery.
		/// </summary>
		public async Task<GenericResponseDTO<List<ListarCadenaDTO>>> Handle(ListarCadenasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarCadenaDTO>> { Ok = true, Data = new List<ListarCadenaDTO>() };

			try
			{
				var cadenas = await _contexto.RepositorioMaeCadena.Obtener(x => x.CodEmpresa == request.CodEmpresa).ToListAsync();
				var cadenasAsociadas = await _contexto.RepositorioSegCadena.Obtener(x => x.CodUsuario == request.CodUsuario && x.CodEmpresa == request.CodEmpresa).ToListAsync();

				var cadenasDto = _mapper.Map<List<ListarCadenaDTO>>(cadenas);

				foreach (var item in cadenasDto)
				{
					item.IndAsociado = cadenasAsociadas.Exists(x => x.CodCadena == item.CodCadena);
				}

				response.Data = cadenasDto.OrderBy(x => x.CodEmpresa).ToList();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las cadenas";
				_logger.Error(ex, response.Mensaje);
			}

			return response;
		}
	}
}
