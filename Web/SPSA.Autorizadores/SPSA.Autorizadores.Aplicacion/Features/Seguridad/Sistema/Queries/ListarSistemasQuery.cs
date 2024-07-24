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
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Queries
{
	/// <summary>
	/// Query para listar los sistemas.
	/// </summary>
	public class ListarSistemasQuery : IRequest<GenericResponseDTO<List<ListarSistemaDTO>>>
	{
	}

	public class ListarSistemasHandler : IRequestHandler<ListarSistemasQuery, GenericResponseDTO<List<ListarSistemaDTO>>>
	{
		private readonly ISGPContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;


		public ListarSistemasHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new SGPContexto();
		}

		/// <summary>
		/// Maneja la solicitud para listar los sistemas.
		/// </summary>
		/// <param name="request">La solicitud de listar sistemas.</param>
		/// <param name="cancellationToken">El token de cancelación.</param>
		/// <returns>La respuesta con la lista de sistemas.</returns>
		public async Task<GenericResponseDTO<List<ListarSistemaDTO>>> Handle(ListarSistemasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarSistemaDTO>> { Ok = true };
			try
			{
				var sistema = await _contexto.RepositorioSegSistema.Obtener().ToListAsync();
				var sistemaDto = _mapper.Map<List<ListarSistemaDTO>>(sistema);
				response.Data = sistemaDto;
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar los sistemas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
