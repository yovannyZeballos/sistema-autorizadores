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

namespace SPSA.Autorizadores.Aplicacion.Features.Regiones.Queries
{
	public class ListarRegionesAsociadasQuery : IRequest<GenericResponseDTO<List<ListarRegionDTO>>>
	{
		public string CodUsuario { get; set; }
		public string CodEmpresa { get; set; }
		public string CodCadena { get; set; }
	}

	public class ListarRegionesAsociadasHandler : IRequestHandler<ListarRegionesAsociadasQuery, GenericResponseDTO<List<ListarRegionDTO>>>
	{
		private readonly IMapper _mapper;
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		public ListarRegionesAsociadasHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		public async Task<GenericResponseDTO<List<ListarRegionDTO>>> Handle(ListarRegionesAsociadasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarRegionDTO>> { Ok = true, Data = new List<ListarRegionDTO>() };

			try
			{
				var Regiones = await _contexto.RepositorioSegRegion
					.Obtener(x => x.CodUsuario == request.CodUsuario 
							&& x.CodEmpresa == request.CodEmpresa
							&& x.CodCadena == request.CodCadena)
					.Include(x => x.Mae_Region)
                    .OrderBy(x => x.CodRegion)
                    .ToListAsync();

				response.Data = _mapper.Map<List<ListarRegionDTO>>(Regiones);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las Regiones asociadas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
