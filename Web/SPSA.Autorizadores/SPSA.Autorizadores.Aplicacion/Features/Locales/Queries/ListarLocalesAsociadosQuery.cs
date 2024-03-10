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

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{
	public class ListarLocalesAsociadasQuery : IRequest<GenericResponseDTO<List<ListarLocalDTO>>>
	{
		public string CodUsuario { get; set; }
		public string CodEmpresa { get; set; }
		public string CodCadena { get; set; }
		public string CodRegion { get; set; }
		public string CodZona { get; set; }
	}

	public class ListarLocalesAsociadasHandler : IRequestHandler<ListarLocalesAsociadasQuery, GenericResponseDTO<List<ListarLocalDTO>>>
	{
		private readonly IMapper _mapper;
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		public ListarLocalesAsociadasHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		public async Task<GenericResponseDTO<List<ListarLocalDTO>>> Handle(ListarLocalesAsociadasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarLocalDTO>> { Ok = true, Data = new List<ListarLocalDTO>() };

			try
			{
				var Locales = await _contexto.RepositorioSegLocal
					.Obtener(x => x.CodUsuario == request.CodUsuario
							&& x.CodEmpresa == request.CodEmpresa
							&& x.CodCadena == request.CodCadena
							&& x.CodZona == request.CodZona
							&& x.CodRegion == request.CodRegion)
					.Include(x => x.Mae_Local)
					.ToListAsync();

				response.Data = _mapper.Map<List<ListarLocalDTO>>(Locales);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las Locales asociadas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
