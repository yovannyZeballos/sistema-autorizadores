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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cadenas.Queries
{
	public class ListarCadenasAsociadasQuery : IRequest<GenericResponseDTO<List<ListarCadenaDTO>>>
	{
		public string CodUsuario { get; set; }
		public string CodEmpresa { get; set; }
		public string Busqueda { get; set; }
	}

	public class ListarCadenasAsociadasHandler : IRequestHandler<ListarCadenasAsociadasQuery, GenericResponseDTO<List<ListarCadenaDTO>>>
	{
		private readonly IMapper _mapper;
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		public ListarCadenasAsociadasHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		public async Task<GenericResponseDTO<List<ListarCadenaDTO>>> Handle(ListarCadenasAsociadasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarCadenaDTO>> { Ok = true, Data = new List<ListarCadenaDTO>() };

			try
			{
				var busqueda = (request.Busqueda ?? "").ToUpper();
				var cadenas = await _contexto.RepositorioSegCadena
					.Obtener(x => x.CodUsuario == request.CodUsuario 
							&& x.CodEmpresa == request.CodEmpresa
							&& (busqueda == "" || x.Mae_Cadena.NomCadena.ToUpper().Contains(busqueda)))
					.Include(x => x.Mae_Cadena)
					.ToListAsync();

				response.Data = _mapper.Map<List<ListarCadenaDTO>>(cadenas);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las Cadenas asociadas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
