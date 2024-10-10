using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
	/// <summary>
	/// Query para obtener los registros del monitor BCT.
	/// </summary>
	public class ObtenerRegistrosMonitorBCTQuery : IRequest<ObtenerComunDTO<TransactionXmlCT2>>
	{
		/// <summary>
		/// Código de la empresa.
		/// </summary>
		public string CodEmpresa { get; set; }
	}

	/// <summary>
	/// Manejador de la query para obtener los registros del monitor BCT.
	/// </summary>
	public class ObtenerRegistrosMonitorBCTHandler : IRequestHandler<ObtenerRegistrosMonitorBCTQuery, ObtenerComunDTO<TransactionXmlCT2>>
	{
		private readonly IRepositorioTransactionXmlCT2 _repositorioTransactionXmlCT2;
		private readonly ISGPContexto _contexto;

		public ObtenerRegistrosMonitorBCTHandler(IRepositorioTransactionXmlCT2 repositorioTransactionXmlCT2)
		{
			_repositorioTransactionXmlCT2 = repositorioTransactionXmlCT2;
			_contexto = new SGPContexto();
		}

		/// <summary>
		/// Maneja la query para obtener los registros del monitor BCT.
		/// </summary>
		/// <param name="request">La query para obtener los registros del monitor BCT.</param>
		/// <param name="cancellationToken">Token de cancelación.</param>
		/// <returns>Los registros del monitor BCT.</returns>
		public async Task<ObtenerComunDTO<TransactionXmlCT2>> Handle(ObtenerRegistrosMonitorBCTQuery request, CancellationToken cancellationToken)
		{
			var response = new ObtenerComunDTO<TransactionXmlCT2> { Ok = true };

			try
			{
				string cadenaConexion = await ArmarCadenaconexion(request.CodEmpresa);
				string nombreTabla = await ObtenerNombreTabla(request.CodEmpresa);

				if (cadenaConexion == null) return response;

				response.Data = await _repositorioTransactionXmlCT2.Obtener(cadenaConexion, nombreTabla);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}

			return response;
		}

		/// <summary>
		/// Arma la cadena de conexión para la empresa especificada.
		/// </summary>
		/// <param name="codEmpresa">El código de la empresa.</param>
		/// <returns>La cadena de conexión.</returns>
		private async Task<string> ArmarCadenaconexion(string codEmpresa)
		{
			var parametros = await _contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoBct &&
																								 x.CodEmpresa == codEmpresa).ToListAsync();

			var servidor = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroServidorBD).Select(x => x.ValParametro).FirstOrDefault();
			var instancia = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroInstanciaBD).Select(x => x.ValParametro).FirstOrDefault();
			var nombreBD = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroNombreBD).Select(x => x.ValParametro).FirstOrDefault();
			var usuario = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroUsuarioBD).Select(x => x.ValParametro).FirstOrDefault();
			var clave = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroClaveBD).Select(x => x.ValParametro).FirstOrDefault();

			if (string.IsNullOrEmpty(servidor) || string.IsNullOrEmpty(instancia) || string.IsNullOrEmpty(nombreBD) ||
				string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
			{
				return null;
			}

			return $"Server={servidor}\\{instancia};Initial Catalog={nombreBD};Persist Security Info=False;" +
				$"User ID={usuario};Password={clave};MultipleActiveResultSets=False;Connection Timeout=30";
		}

		/// <summary>
		/// Obtiene el nombre de la tabla para la empresa especificada.
		/// </summary>
		/// <param name="codEmpresa">El código de la empresa.</param>
		/// <returns>El nombre de la tabla.</returns>
		private async Task<string> ObtenerNombreTabla(string codEmpresa)
		{
			var parametros = await _contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoBct &&
																								 x.CodEmpresa == codEmpresa).ToListAsync();
			return parametros.Where(x => x.CodParametro == Constantes.CodigoParametroNombreTabla).Select(x => x.ValParametro).FirstOrDefault();
		}
	}
}
