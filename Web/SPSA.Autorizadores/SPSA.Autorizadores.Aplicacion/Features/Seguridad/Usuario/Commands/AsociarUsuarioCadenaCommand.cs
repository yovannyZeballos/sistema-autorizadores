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
	/// Comando para asociar un usuario a una cadena.
	/// </summary>
	public class AsociarUsuarioCadenaCommand : IRequest<RespuestaComunDTO>
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
		/// Cadenas a asociar al usuario.
		/// </summary>
		public string[] CadenasAsociadas { get; set; }
	}

	/// <summary>
	/// Manejador del comando AsociarUsuarioCadenaCommand.
	/// </summary>
	public class AsociarUsuarioCadenaHandler : IRequestHandler<AsociarUsuarioCadenaCommand, RespuestaComunDTO>
	{
		private readonly ISGPContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor del manejador.
		/// </summary>
		public AsociarUsuarioCadenaHandler()
		{
			_contexto = new SGPContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioCadenaCommand.
		/// </summary>
		/// <param name="request">El comando.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Una tarea que representa la operación asincrónica. El resultado de la tarea es una RespuestaComunDTO.</returns>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioCadenaCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
            var cadenasAgregar = new List<Seg_Cadena>();
            var cadenasEliminar = new List<Seg_Cadena>();
            try
			{
				var cadenasAsocidas = await _contexto.RepositorioSegCadena
					.Obtener(x => x.CodUsuario == request.CodUsuario && x.CodEmpresa == request.CodEmpresa)
					.ToListAsync();

                HashSet<string> codigosHashSet = new HashSet<string>();
                foreach (var cadena in cadenasAsocidas)
                {
                    codigosHashSet.Add(cadena.CodCadena);
                }

                // Identificar los códigos para agregar
                foreach (var codigo in request.CadenasAsociadas)
                {
                    if (!codigosHashSet.Contains(codigo))
                    {
                        cadenasAgregar.Add(new Seg_Cadena
                        {
                            CodUsuario = request.CodUsuario,
                            CodEmpresa = request.CodEmpresa,
                            CodCadena = codigo
						});
                    }
                }

                // Identificar los códigos para eliminaren cascada
                foreach (var codigo in codigosHashSet)
                {
                    if (!Array.Exists(request.CadenasAsociadas, c => c == codigo))
                    {
                        cadenasEliminar.Add(new Seg_Cadena
                        {
                            CodUsuario = request.CodUsuario,
                            CodEmpresa = request.CodEmpresa,
                            CodCadena = codigo
						});
                    }
                }

                if (cadenasEliminar.Count > 0)
				{
                    foreach (var cadena in cadenasEliminar)
                    {
                        var cadenaDesasociada = await _contexto.RepositorioSegCadena.Obtener(e => e.CodUsuario == cadena.CodUsuario && e.CodEmpresa == cadena.CodEmpresa && e.CodCadena == cadena.CodCadena).FirstOrDefaultAsync();
                        _contexto.RepositorioSegCadena.Eliminar(cadenaDesasociada);
                    }
				}

				if (cadenasAgregar.Count > 0)
				{
					foreach (var cadena in cadenasAgregar)
					{
						_contexto.RepositorioSegCadena.Agregar(cadena);
					}


					await _contexto.GuardarCambiosAsync();
				}
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar las cadenas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}