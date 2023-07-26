using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Queries
{
	public class EliminarInventarioServidorVirtualCommand : IRequest<RespuestaComunDTO>
	{
        public string Ids { get; set; }
    }

	public class EliminarInventarioServidorVirtualHandler : IRequestHandler<EliminarInventarioServidorVirtualCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioInventarioServidorVirtual _repositorioInventarioServidorVirtual;

		public EliminarInventarioServidorVirtualHandler(IRepositorioInventarioServidorVirtual repositorioInventarioServidorVirtual)
		{
			_repositorioInventarioServidorVirtual = repositorioInventarioServidorVirtual;
		}

		public async Task<RespuestaComunDTO> Handle(EliminarInventarioServidorVirtualCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO();

			try
			{
				await _repositorioInventarioServidorVirtual.EliminarPorIds(request.Ids);
				respuesta.Ok = true;
				respuesta.Mensaje = "Virtuales eliminadas exitosamente";
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return respuesta;
		}
	}
}
