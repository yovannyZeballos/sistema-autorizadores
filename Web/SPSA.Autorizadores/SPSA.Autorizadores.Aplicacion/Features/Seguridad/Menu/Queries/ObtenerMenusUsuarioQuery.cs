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

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Queries
{
	public class ObtenerMenusUsuarioQuery : IRequest<GenericResponseDTO<List<ListarMenuDTO>>>
	{
		public string CodUsuario { get; set; }
		public string SiglaSistema { get; set; }
	}

	public class ObtenerMenusUsuarioHandler : IRequestHandler<ObtenerMenusUsuarioQuery, GenericResponseDTO<List<ListarMenuDTO>>>
	{
		private readonly ISGPContexto _bCTContexto;
		private readonly ILogger _logger;
		private readonly IMapper _mapper;

		public ObtenerMenusUsuarioHandler(IMapper mapper)
		{
			_bCTContexto = new SGPContexto();
			_logger = SerilogClass._log;
			_mapper = mapper;

		}

		public async Task<GenericResponseDTO<List<ListarMenuDTO>>> Handle(ObtenerMenusUsuarioQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarMenuDTO>> { Ok = true };
			try
			{
				var sistema = await _bCTContexto.RepositorioSegSistema.Obtener(x => x.Sigla == request.SiglaSistema).FirstOrDefaultAsync();
				var perfiles = await _bCTContexto.RepositorioSegPerfilUsuario.Obtener(x => x.CodUsuario == request.CodUsuario).Include(x => x.Seg_Perfil).ToListAsync();
				var menusPerfiles = await _bCTContexto.RepositorioSegPerfilMenu.Obtener(x => x.CodSistema == sistema.CodSistema && 
											perfiles.Select(y => y.CodPerfil).Contains(x.CodPerfil)).Select(x => x.CodMenu).Distinct().ToListAsync();

				var menus = await _bCTContexto.RepositorioSegMenu.Obtener(x => x.CodSistema == sistema.CodSistema && menusPerfiles.Contains(x.CodMenu)).ToListAsync();

				response.Data = _mapper.Map<List<ListarMenuDTO>>(menus);

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
