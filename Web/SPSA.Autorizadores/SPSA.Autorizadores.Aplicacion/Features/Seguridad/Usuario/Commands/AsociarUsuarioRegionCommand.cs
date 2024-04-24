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
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands
{
	/// <summary>
	/// Comando para asociar un usuario a una region.
	/// </summary>
	public class AsociarUsuarioRegionCommand : IRequest<RespuestaComunDTO>
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
		/// Regions a asociar al usuario.
		/// </summary>
		public string[] RegionesAsociadas { get; set; }
	}

	/// <summary>
	/// Manejador del comando AsociarUsuarioRegionCommand.
	/// </summary>
	public class AsociarUsuarioRegionHandler : IRequestHandler<AsociarUsuarioRegionCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor del manejador.
		/// </summary>
		public AsociarUsuarioRegionHandler()
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioRegionCommand.
		/// </summary>
		/// <param name="request">El comando.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Una tarea que representa la operación asincrónica. El resultado de la tarea es una RespuestaComunDTO.</returns>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioRegionCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
            var regionesAgregar = new List<Seg_Region>();
            var regionesEliminar = new List<Seg_Region>();
            try
			{
				var regionesAsocidas = await _contexto.RepositorioSegRegion
					.Obtener(x => x.CodUsuario == request.CodUsuario && x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena)
					.ToListAsync();

                HashSet<string> codigosHashSet = new HashSet<string>();
                foreach (var region in regionesAsocidas)
                {
                    codigosHashSet.Add(region.CodRegion);
                }

                // Identificar los códigos para agregar
                foreach (var codigo in request.RegionesAsociadas)
                {
                    if (!codigosHashSet.Contains(codigo))
                    {
                        regionesAgregar.Add(new Seg_Region
                        {
                            CodUsuario = request.CodUsuario,
                            CodEmpresa = request.CodEmpresa,
                            CodCadena = request.CodCadena,
                            CodRegion = codigo
                        });
                    }
                }

                // Identificar los códigos para eliminaren cascada
                foreach (var codigo in codigosHashSet)
                {
                    if (!Array.Exists(request.RegionesAsociadas, c => c == codigo))
                    {
                        regionesEliminar.Add(new Seg_Region
                        {
                            CodUsuario = request.CodUsuario,
                            CodEmpresa = request.CodEmpresa,
                            CodCadena = request.CodCadena,
                            CodRegion = codigo
                        });
                    }
                }

                if (regionesEliminar.Count > 0)
                {
                    foreach (var cadena in regionesEliminar)
                    {
                        var regionDesasociada = await _contexto.RepositorioSegRegion.Obtener(e => e.CodUsuario == cadena.CodUsuario && e.CodEmpresa == cadena.CodEmpresa 
																							&& e.CodCadena == cadena.CodCadena && e.CodRegion == cadena.CodRegion).FirstOrDefaultAsync();
                        _contexto.RepositorioSegRegion.Eliminar(regionDesasociada);
                    }
                }

                if (regionesAgregar.Count > 0)
                {
                    foreach (var cadena in regionesAgregar)
                    {
                        _contexto.RepositorioSegRegion.Agregar(cadena);
                    }
                }
                await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar las regiones";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}