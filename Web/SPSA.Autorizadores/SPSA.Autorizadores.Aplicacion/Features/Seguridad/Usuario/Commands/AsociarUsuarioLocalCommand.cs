using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands
{
	/// <summary>
	/// Comando para asociar un usuario a una local.
	/// </summary>
	public class AsociarUsuarioLocalCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código del usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }

		/// <summary>
		/// Código de la cadena.
		/// </summary>
		public string CodCadena { get; set; }

		/// <summary>
		/// Código de la region.
		/// </summary>
		public string CodRegion { get; set; }

		/// <summary>
		/// Código de la zona.
		/// </summary>
		public string CodZona { get; set; }

		/// <summary>
		/// Locales a asociar al usuario.
		/// </summary>
		public string[] LocalesAsociados { get; set; }
	}

	/// <summary>
	/// Manejador del comando AsociarUsuarioLocalCommand.
	/// </summary>
	public class AsociarUsuarioLocalHandler : IRequestHandler<AsociarUsuarioLocalCommand, RespuestaComunDTO>
	{
		private readonly ISGPContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor del manejador.
		/// </summary>
		public AsociarUsuarioLocalHandler()
		{
			_contexto = new SGPContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioLocalCommand.
		/// </summary>
		/// <param name="request">El comando.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Una tarea que representa la operación asincrónica. El resultado de la tarea es una RespuestaComunDTO.</returns>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioLocalCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
            var localesAgregar = new List<Seg_Local>();
            var localesEliminar = new List<Seg_Local>();
            try
			{
				var localesAsocidas = await _contexto.RepositorioSegLocal
					.Obtener(x => x.CodUsuario == request.CodUsuario 
					&& x.CodEmpresa == request.CodEmpresa 
					&& x.CodCadena == request.CodCadena
					&& x.CodRegion == request.CodRegion
					&& x.CodZona == request.CodZona)
					.ToListAsync();

                HashSet<string> codigosHashSet = new HashSet<string>();
                foreach (var local in localesAsocidas)
                {
                    codigosHashSet.Add(local.CodLocal);
                }

                // Identificar los códigos para agregar
                foreach (var codigo in request.LocalesAsociados)
                {
                    if (!codigosHashSet.Contains(codigo))
                    {
                        localesAgregar.Add(new Seg_Local
                        {
                            CodUsuario = request.CodUsuario,
                            CodEmpresa = request.CodEmpresa,
                            CodCadena = request.CodCadena,
                            CodRegion = request.CodRegion,
                            CodZona = request.CodZona,
                            CodLocal = codigo
                        });
                    }
                }

                // Identificar los códigos para eliminaren cascada
                foreach (var codigo in codigosHashSet)
                {
                    if (!Array.Exists(request.LocalesAsociados, c => c == codigo))
                    {
                        localesEliminar.Add(new Seg_Local
                        {
                            CodUsuario = request.CodUsuario,
                            CodEmpresa = request.CodEmpresa,
                            CodCadena = request.CodCadena,
                            CodRegion = request.CodRegion,
                            CodZona = request.CodZona,
                            CodLocal = codigo
                        });
                    }
                }

                if (localesEliminar.Count > 0)
                {
                    foreach (var local in localesEliminar)
                    {
                        var localDesasociada = await _contexto.RepositorioSegLocal.Obtener(e => e.CodUsuario == local.CodUsuario && e.CodEmpresa == local.CodEmpresa
                                                                                            && e.CodCadena == local.CodCadena && e.CodRegion == local.CodRegion
                                                                                            && e.CodZona == local.CodZona && e.CodLocal == local.CodLocal).FirstOrDefaultAsync();
                        _contexto.RepositorioSegLocal.Eliminar(localDesasociada);
                    }
                }

                if (localesAgregar.Count > 0)
                {
                    foreach (var local in localesAgregar)
                    {
                        _contexto.RepositorioSegLocal.Agregar(local);
                    }
                }

				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar los locals";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}