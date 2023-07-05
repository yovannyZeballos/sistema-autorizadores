using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries
{
	public class ObtenerInventarioServidorQuery : IRequest<InventarioServidorDTO>
	{
        public string CodEmpresa { get; set; }
        public string CodFormato { get; set; }
        public string CodLocal { get; set; }
        public string NumServer { get; set; }
    }

	public class ObtenerInventarioServidorHandler : IRequestHandler<ObtenerInventarioServidorQuery, InventarioServidorDTO>
	{
		private readonly IMapper _mapper;
		private readonly IRepositorioInventarioServidor _repositorioInventarioServidor;

		public ObtenerInventarioServidorHandler(IMapper mapper, IRepositorioInventarioServidor repositorioInventarioServidor)
		{
			_mapper = mapper;
			_repositorioInventarioServidor = repositorioInventarioServidor;
		}

		public async Task<InventarioServidorDTO> Handle(ObtenerInventarioServidorQuery request, CancellationToken cancellationToken)
		{
			var inventarioDto = new InventarioServidorDTO();
			try
			{
				var inventario = await _repositorioInventarioServidor.Obtener(request.CodEmpresa, request.CodFormato, request.CodLocal, request.NumServer);
				inventarioDto = _mapper.Map<InventarioServidorDTO>(inventario);
				inventarioDto.Ok = true;
			}
			catch (Exception ex)
			{
				inventarioDto.Ok = false;
				inventarioDto.Mensaje = ex.Message;
			}
			return inventarioDto;
		}
	}
}
