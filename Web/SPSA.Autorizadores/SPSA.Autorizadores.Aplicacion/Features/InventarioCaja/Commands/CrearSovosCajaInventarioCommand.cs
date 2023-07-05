using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
	public class CrearSovosCajaInventarioCommand : IRequest<RespuestaComunDTO>
	{
		public string CodEmpresa { get; set; }
		public string CodFormato { get; set; }
		public string CodLocal { get; set; }
		public decimal NumPos { get; set; }
		public string Ranking { get; set; }
		public string Estado { get; set; }
		public string Sede { get; set; }
		public string Ubicacion { get; set; }
		public string Caja { get; set; }
		public string ModeloCpu { get; set; }
		public string Serie { get; set; }
		public string ModeloPrint { get; set; }
		public string SeriePrint { get; set; }
		public string ModeloDinakey { get; set; }
		public string SerieDinakey { get; set; }
		public string ModeloScanner { get; set; }
		public string SerieScanner { get; set; }
		public string ModeloGaveta { get; set; }
		public string SerieGaveta { get; set; }
		public string ModeloMonitor { get; set; }
		public string SerieMonitor { get; set; }
		public DateTime? FechaApertura { get; set; }
		public string Caract1 { get; set; }
		public string Caract2 { get; set; }
		public string Caract3 { get; set; }
		public DateTime? FechaLising { get; set; }
		public string So { get; set; }
		public string VesionSo { get; set; }
		public DateTime? FechaAsignacion { get; set; }
		public string Usuario { get; set; }
	}

	public class CrearSovosCajaInventarioHandler : IRequestHandler<CrearSovosCajaInventarioCommand, RespuestaComunDTO>
	{
		private readonly IRepositorioSovosInventarioCaja _repositorioSovosInventarioCaja;

		public CrearSovosCajaInventarioHandler(IRepositorioSovosInventarioCaja repositorioSovosInventarioCaja)
		{
			_repositorioSovosInventarioCaja = repositorioSovosInventarioCaja;
		}

		public async Task<RespuestaComunDTO> Handle(CrearSovosCajaInventarioCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunDTO();

			try
			{
				await _repositorioSovosInventarioCaja.Insertar(new SovosCajaInventario(request.CodEmpresa, request.CodFormato, request.CodLocal, request.NumPos,
				request.Ranking, request.Estado, request.Sede, request.Ubicacion, request.Caja, request.ModeloCpu, request.Serie, request.ModeloPrint,
				request.SeriePrint, request.ModeloDinakey, request.SerieDinakey, request.ModeloScanner, request.SerieScanner, request.ModeloGaveta, request.SerieGaveta,
				request.ModeloMonitor, request.SerieMonitor, request.FechaApertura, request.Caract1, request.Caract2, request.Caract3, request.FechaLising, request.So,
				request.VesionSo, request.FechaAsignacion, request.Usuario));

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
