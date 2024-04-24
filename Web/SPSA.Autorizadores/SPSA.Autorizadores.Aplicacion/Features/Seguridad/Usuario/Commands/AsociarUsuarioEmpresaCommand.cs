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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands
{
	/// <summary>
	/// Comando para asociar un usuario con una empresa.
	/// </summary>
	public class AsociarUsuarioEmpresaCommand : IRequest<RespuestaComunDTO>
	{
		/// <summary>
		/// Código de usuario.
		/// </summary>
		public string CodUsuario { get; set; }

		/// <summary>
		/// Empresas asociadas.
		/// </summary>
		public string[] EmpresasAsociadas { get; set; }
	}

	/// <summary>
	/// Manejador para el comando AsociarUsuarioEmpresaCommand.
	/// </summary>
	public class AsociarUsuarioEmpresaHandler : IRequestHandler<AsociarUsuarioEmpresaCommand, RespuestaComunDTO>
	{
		private readonly IBCTContexto _contexto;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor para el manejador AsociarUsuarioEmpresaHandler.
		/// </summary>
		public AsociarUsuarioEmpresaHandler()
		{
			_contexto = new BCTContexto();
			_logger = SerilogClass._log;
		}

		/// <summary>
		/// Maneja el comando AsociarUsuarioEmpresaCommand.
		/// </summary>
		public async Task<RespuestaComunDTO> Handle(AsociarUsuarioEmpresaCommand request, CancellationToken cancellationToken)
		{
			var response = new RespuestaComunDTO { Ok = true };
			var empresasAgregar = new List<Seg_Empresa>();
			var empresasEliminar = new List<Seg_Empresa>();
			try
			{
				var empresasAsocidas = await _contexto.RepositorioSegEmpresa.Obtener(x => x.CodUsuario == request.CodUsuario).ToListAsync();
				//var listaEmpresas = await _contexto.RepositorioMaeEmpresa.Obtener().ToListAsync();

                HashSet<string> codigosHashSet = new HashSet<string>();
                foreach (var empresa in empresasAsocidas)
                {
                    codigosHashSet.Add(empresa.CodEmpresa);
                }

                // Identificar los códigos para agregar
                foreach (var codigo in request.EmpresasAsociadas)
                {
                    if (!codigosHashSet.Contains(codigo))
                    {
                        empresasAgregar.Add(new Seg_Empresa
                        {
                            CodUsuario = request.CodUsuario,
                            CodEmpresa = codigo,
							//Mae_Empresa = listaEmpresas.Where(x=>x.CodEmpresa == codigo).FirstOrDefault(),
                        });
                    }
                }

				// Identificar los códigos para eliminaren cascada
				foreach (var codigo in codigosHashSet)
				{
					if (!Array.Exists(request.EmpresasAsociadas, c => c == codigo))
					{
                        empresasEliminar.Add(new Seg_Empresa
                        {
                            CodUsuario = request.CodUsuario,
                            CodEmpresa = codigo,
                            //Mae_Empresa = listaEmpresas.Where(x => x.CodEmpresa == codigo).FirstOrDefault(),
                        });
                    }
				}


				if (empresasEliminar.Count > 0)
				{
					foreach (var empresa in empresasEliminar)
					{
                        var empresaDesasociada = await _contexto.RepositorioSegEmpresa.Obtener(e => e.CodUsuario == empresa.CodUsuario && e.CodEmpresa == empresa.CodEmpresa).FirstOrDefaultAsync();
                        _contexto.RepositorioSegEmpresa.Eliminar(empresaDesasociada);
					}
				}

				if (empresasAgregar.Count > 0)
				{
					foreach (var empresa in empresasAgregar)
					{
						_contexto.RepositorioSegEmpresa.Agregar(empresa);
					}
				}

				await _contexto.GuardarCambiosAsync();
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = "Ocurrió un error al asociar las empresas";
				_logger.Error(ex, response.Mensaje);
			}
			return response;
		}
	}
}
