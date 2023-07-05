using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Commands
{
	public class CrearInventarioServidorVirtualCommand : IRequest<RespuestaComunDTO>
	{
		public int Id { get; set; }
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
		public string NumServer { get; set; }
		public string Tipo { get; set; }
		public decimal Ram { get; set; }
		public decimal Cpu { get; set; }
		public decimal Hdd { get; set; }
		public string So { get; set; }
		public string Usuario { get; set; }
	}

	public class CrearInventarioServidorVirtualHandler : IRequestHandler<CrearInventarioServidorVirtualCommand, RespuestaComunDTO>
	{
		readonly IRepositorioInventarioServidorVirtual _repositorioInventarioServidorVirtual;

		public CrearInventarioServidorVirtualHandler(IRepositorioInventarioServidorVirtual repositorioInventarioServidorVirtual)
		{
			_repositorioInventarioServidorVirtual = repositorioInventarioServidorVirtual;
		}

		public async Task<RespuestaComunDTO> Handle(CrearInventarioServidorVirtualCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO();

			try
			{
				await _repositorioInventarioServidorVirtual.Insertar(new InventarioServidorVirtual(request.Id, request.CodEmpresa, request.CodFormato, request.CodLocal, request.NumServer,
					request.Tipo, request.Ram, request.Cpu, request.Hdd, request.So, request.Usuario));

				respuesta.Ok = true;
				respuesta.Mensaje = "Registro guardado exitosamente";
			}
			catch (Exception ex)
			{
				respuesta.Mensaje = ex.Message;
				respuesta.Ok = false;
			}

			return respuesta;
		}
	}
}
