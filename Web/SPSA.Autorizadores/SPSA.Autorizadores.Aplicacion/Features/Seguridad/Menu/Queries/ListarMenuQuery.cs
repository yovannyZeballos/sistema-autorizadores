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
	public class ListarMenuQuery: IRequest<GenericResponseDTO<List<ListarMenuDTO>>>
	{
        public string CodSistema { get; set; }
    }

    public class ListarMenuHandler : IRequestHandler<ListarMenuQuery, GenericResponseDTO<List<ListarMenuDTO>>>
	{
		private readonly IBCTContexto _contexto;
		private readonly IMapper _mapper;
		private readonly ILogger _logger = SerilogClass._log;


		public ListarMenuHandler(IMapper mapper)
		{
			_mapper = mapper;
			_contexto = new BCTContexto();
		}

		public async Task<GenericResponseDTO<List<ListarMenuDTO>>> Handle(ListarMenuQuery request, CancellationToken cancellationToken)
		{
			var response = new GenericResponseDTO<List<ListarMenuDTO>> { Ok = true };
			try
			{
				var menus = await _contexto.RepositorioSegMenu.Obtener(x => x.CodSistema == request.CodSistema).ToListAsync();
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
