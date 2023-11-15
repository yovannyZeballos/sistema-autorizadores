using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajeroVolante.Commands
{
	public class CrearCajeroVolanteCommand : IRequest<RespuestaComunDTO>
	{
		public string CodOfisis { get; set; }
		public string NumDocumento { get; set; }
		public string CodEmpresaOrigen { get; set; }
		public decimal CodSedeOrigen { get; set; }
		public List<string> LocalesAsignados { get; set; }
		public string CodEmpresa { get; set; }
		public string Coordinador { get; set; }
		public string Usuario { get; set; }
	}

	public class CrearCajeroVolanteHandler : IRequestHandler<CrearCajeroVolanteCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioCajero _repositorioCajero;

		public CrearCajeroVolanteHandler(IRepositorioCajero repositorioCajero)
		{
			_repositorioCajero = repositorioCajero;
		}

		public async Task<RespuestaComunDTO> Handle(CrearCajeroVolanteCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO
			{
				Ok = true
			};

			var sb = new StringBuilder();

			foreach (var local in request.LocalesAsignados)
			{
				try
				{
					await _repositorioCajero.CrearCajeroVolante(new CajeroVolante(request.CodOfisis, request.NumDocumento, request.CodEmpresaOrigen, request.CodSedeOrigen,
						request.CodEmpresa, Convert.ToInt32(local), request.Coordinador, request.Usuario));
				}
				catch (Exception ex)
				{
					sb.AppendLine($"Error al crear el cajero: {request.CodOfisis} para el local {local} | {ex.Message}");
					respuesta.Ok = false;
				}
			}

			respuesta.Mensaje = sb.ToString();
			return respuesta;
		}
	}
}
