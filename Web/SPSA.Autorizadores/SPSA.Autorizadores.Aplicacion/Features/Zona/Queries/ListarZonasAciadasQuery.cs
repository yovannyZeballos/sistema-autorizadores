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

namespace SPSA.Autorizadores.Aplicacion.Features.Zona.Queries
{
	public class ListarZonasAsociadasQuery : IRequest<GenericResponseDTO<List<ListarZonaDTO>>>
	{
		public string CodUsuario { get; set; }
		public string CodEmpresa { get; set; }
		public string CodCadena { get; set; }
		public string CodRegion { get; set; }
	}

	public class ListarZonasAsociadasHandler : IRequestHandler<ListarZonasAsociadasQuery, GenericResponseDTO<List<ListarZonaDTO>>>
	{
		private readonly IMapper _mapper;
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		public ListarZonasAsociadasHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		public async Task<GenericResponseDTO<List<ListarZonaDTO>>> Handle(ListarZonasAsociadasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarZonaDTO>> { Ok = true, Data = new List<ListarZonaDTO>() };

			try
			{
				var Zonas = await _contexto.RepositorioSegZona
					.Obtener(x => x.CodUsuario == request.CodUsuario
							&& x.CodEmpresa == request.CodEmpresa
							&& x.CodCadena == request.CodCadena
							&& x.CodRegion == request.CodRegion)
					.Include(x => x.Mae_Zona)
                    .OrderBy(x => x.CodZona)
                    .ToListAsync();

				response.Data = _mapper.Map<List<ListarZonaDTO>>(Zonas);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las Zonas asociadas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
