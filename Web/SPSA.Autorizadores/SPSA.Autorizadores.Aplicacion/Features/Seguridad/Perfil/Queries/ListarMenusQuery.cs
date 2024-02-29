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

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Queries
{
	public class ListarMenusQuery : IRequest<GenericResponseDTO<List<ListarMenuDTO>>>
	{
		public string CodSistema { get; set; }
		public string CodPerfil { get; set; }
	}

	public class ListarMenusHandler : IRequestHandler<ListarMenusQuery, GenericResponseDTO<List<ListarMenuDTO>>>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;


		public ListarMenusHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
		}

		public async Task<GenericResponseDTO<List<ListarMenuDTO>>> Handle(ListarMenusQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarMenuDTO>> { Ok = true };
			try
			{
				var menus = await _contexto.RepositorioSegMenu
					.Obtener(x => x.CodSistema == request.CodSistema).ToListAsync();

				var menusAsociados = await _contexto.RepositorioSegPerfilMenu
					.Obtener(x => x.CodSistema == request.CodSistema &&
								  x.CodPerfil == request.CodPerfil)
					.ToListAsync();

				var menusDto = _mapper.Map<List<ListarMenuDTO>>(menus);

				foreach (var menu in menusDto)
				{
					if (!string.IsNullOrEmpty(menu.CodMenuPadre) && menu.CodMenuPadre != "0")
						menu.IndAsociado = menusAsociados.Any(x => x.CodMenu == menu.CodMenu);
				}
				response.Data = menusDto;
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al listar los menus";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
