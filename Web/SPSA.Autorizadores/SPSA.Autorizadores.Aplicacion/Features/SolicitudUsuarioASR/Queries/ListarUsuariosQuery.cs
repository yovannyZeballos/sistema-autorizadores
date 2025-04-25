using MediatR;
using Microsoft.IdentityModel.Tokens;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Queries
{
    public class ListarUsuariosQuery : IRequest<GenericResponseDTO<PagedResult<ASR_UsuarioListado>>>
    {
		public string UsuarioLogin { get; set; }
		public string CodLocal { get; set; }
		public string UsuAprobacion { get; set; }
		public string TipAccion { get; set; }
		public string IndAprobado { get; set; }
		public string CodEmpresa { get; set; }
		public int NumeroPagina { get; set; }
		public int TamañoPagina { get; set; }
		public string Busqueda { get; set; }

	}

	public class ListarUsuariosHandler : IRequestHandler<ListarUsuariosQuery, GenericResponseDTO<PagedResult<ASR_UsuarioListado>>>
	{
		public async Task<GenericResponseDTO<PagedResult<ASR_UsuarioListado>>> Handle(ListarUsuariosQuery request, CancellationToken cancellationToken)
		{
			var respuesta = new GenericResponseDTO<PagedResult<ASR_UsuarioListado>> { Ok = true };

			try
			{
				using (ISGPContexto contexto = new SGPContexto())
				{
					var usuarios = await contexto.RepositorioSolicitudUsuarioASR.ListarUsuarios(request.UsuarioLogin ?? "0", 
						request.CodLocal ?? "0", request.UsuAprobacion ?? "0", request.TipAccion, request.IndAprobado, 
						request.CodEmpresa, request.NumeroPagina, request.TamañoPagina);

					respuesta.Data = new PagedResult<ASR_UsuarioListado>
					{
						Items = usuarios.Where(x => x.ToString().ToUpper().Contains((request.Busqueda ?? "").ToUpper())).ToList(),
						PageNumber = request.NumeroPagina,
						PageSize = request.TamañoPagina,
						TotalPages = usuarios.IsNullOrEmpty() ? 0 : usuarios.FirstOrDefault().TotalRegistros,
						TotalRecords = usuarios.IsNullOrEmpty() ? 0 : usuarios.FirstOrDefault().TotalRegistros
					};
				}
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
