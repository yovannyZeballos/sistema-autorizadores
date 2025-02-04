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

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Queries
{
	/// <summary>
	/// Query para listar las empresas asociadas a un usuario.
	/// </summary>
	public class ListarEmpresasAsociadasQuery : IRequest<GenericResponseDTO<List<ListarEmpresaDTO>>>
	{
		/// <summary>
		/// Código del usuario.
		/// </summary>
		public string CodUsuario { get; set; }
		public string Busqueda { get; set; }
	}

	/// <summary>
	/// Manejador para el query ListarEmpresasAsociadasQuery.
	/// </summary>
	public class ListarEmpresasAsociadasHandler : IRequestHandler<ListarEmpresasAsociadasQuery, GenericResponseDTO<List<ListarEmpresaDTO>>>
	{
		private readonly IMapper _mapper;
		private readonly ISGPContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para el manejador ListarEmpresasAsociadasHandler.
		/// </summary>
		public ListarEmpresasAsociadasHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new SGPContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el query ListarEmpresasAsociadasQuery.
		/// </summary>
		public async Task<GenericResponseDTO<List<ListarEmpresaDTO>>> Handle(ListarEmpresasAsociadasQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarEmpresaDTO>> { Ok = true, Data = new List<ListarEmpresaDTO>() };

			try
			{
				var busqueda = (request.Busqueda ?? "").ToUpper();
				var empresas = await _contexto.RepositorioSegEmpresa
					.Obtener(x => x.CodUsuario == request.CodUsuario && (busqueda == "" || x.Mae_Empresa.NomEmpresa.ToUpper().Contains(busqueda)))
					.Include(x => x.Mae_Empresa)
					.ToListAsync();

				response.Data = _mapper.Map<List<ListarEmpresaDTO>>(empresas);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar las empresas asociadas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}