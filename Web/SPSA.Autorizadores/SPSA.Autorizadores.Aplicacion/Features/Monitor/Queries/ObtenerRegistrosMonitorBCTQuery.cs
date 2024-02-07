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
	public class ObtenerRegistrosMonitorBCTQuery : IRequest<ObtenerComunDTO<TransactionXmlCT2>>
	{
		public string CodEmpresa { get; set; }
	}

	public class ObtenerRegistrosMonitorBCTHandler : IRequestHandler<ObtenerRegistrosMonitorBCTQuery, ObtenerComunDTO<TransactionXmlCT2>>
	{
		private readonly IRepositorioTransactionXmlCT2 _repositorioTransactionXmlCT2;
		private readonly IBCTContexto _contexto;

		public ObtenerRegistrosMonitorBCTHandler(IRepositorioTransactionXmlCT2 repositorioTransactionXmlCT2)
		{
			_repositorioTransactionXmlCT2 = repositorioTransactionXmlCT2;
			_contexto = new BCTContexto();
		}

		public async Task<ObtenerComunDTO<TransactionXmlCT2>> Handle(ObtenerRegistrosMonitorBCTQuery request, CancellationToken cancellationToken)
		{
			var response = new ObtenerComunDTO<TransactionXmlCT2> { Ok = true };

			try
			{

				var cadenaConexion = await ArmarCadenaconexion(request.CodEmpresa);

				if (cadenaConexion == null) return response;

				response.Data = await _repositorioTransactionXmlCT2.Obtener(cadenaConexion);
			}
			catch (Exception ex)
			{
				response.Ok = false;
				response.Mensaje = ex.Message;
			}

			return response;
		}

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
				return null;

			return $"Server={servidor}\\{instancia};Initial Catalog={nombreBD};Persist Security Info=False;" +
				$"User ID={usuario};Password={clave};MultipleActiveResultSets=False;Connection Timeout=30";
		}
	}
}
