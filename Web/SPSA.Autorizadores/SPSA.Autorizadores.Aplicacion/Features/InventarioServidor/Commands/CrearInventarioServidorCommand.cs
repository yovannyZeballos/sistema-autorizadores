using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioServidor.Commands
{
	public class CrearInventarioServidorCommand : IRequest<RespuestaComunDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
		public string NumServer { get; set; }
		public string TipoServer { get; set; }
		public string CodMarca { get; set; }
		public string CodModelo { get; set; }
		public string Hostname { get; set; }
		public string Serie { get; set; }
		public string Ip { get; set; }
		public decimal Ram { get; set; }
		public decimal Hdd { get; set; }
		public string CodSo { get; set; }
		public string Replica { get; set; }
		public string IpRemota { get; set; }
		public decimal Antiguedad { get; set; }
		public string Observaciones { get; set; }
		public string Antivirus { get; set; }
		public string Usuario { get; set; }
	}

	public class CrearInventarioServidorHandler : IRequestHandler<CrearInventarioServidorCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioInventarioServidor repositorioInventarioServidor;

		public CrearInventarioServidorHandler(IRepositorioInventarioServidor repositorioInventarioServidor)
		{
			this.repositorioInventarioServidor = repositorioInventarioServidor;
		}

		public async Task<RespuestaComunDTO> Handle(CrearInventarioServidorCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO();

			try
			{
				await repositorioInventarioServidor.Insertar(new Dominio.Entidades.InventarioServidor(request.CodEmpresa, request.CodFormato, request.CodLocal, request.NumServer,
					request.TipoServer, request.CodMarca, request.CodModelo, request.Hostname, request.Serie, request.Ip, request.Ram, request.Hdd, request.CodSo, request.Replica,
					request.IpRemota, request.Antiguedad, request.Observaciones, request.Antivirus, request.Usuario));

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
