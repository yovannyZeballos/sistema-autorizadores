using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
	public class ProcesarMonitorBCTCommand : IRequest<ObtenerComunDTO<(bool, bool, bool, int)>>
	{
		public TransactionXmlCT2 RegistroTotal { get; set; }
		public string FechaAlerta { get; set; }
		public string CodEmpresa { get; set; }
		public int CantidadAnterior { get; set; }
		public int Color { get; set; } //1:Verde, 2:Naranja, 3:Rojo
	}

    public class ProcesarMonitorBCTHandler : IRequestHandler<ProcesarMonitorBCTCommand, ObtenerComunDTO<(bool, bool, bool, int)>>
    {
        private readonly ISGPContexto _contexto;

        public ProcesarMonitorBCTHandler()
        {
            _contexto = new SGPContexto();
        }

        public async Task<ObtenerComunDTO<(bool, bool, bool, int)>> Handle(ProcesarMonitorBCTCommand request, CancellationToken cancellationToken)
        {
            var luzVerde = true;
            var luzNaranja = false;
            var luzRoja = false;
            var envioNotificacion = 0;

            // SPSA-CT3 (12) usa la misma tolerancia que SPSA (02)
            if (request.CodEmpresa == "12")
            {
                request.CodEmpresa = "02";
            }

            var response = new ObtenerComunDTO<(bool, bool, bool, int)> { Ok = true };

            try
            {
                // Obtener parámetros
                var parametros = await _contexto.RepositorioProcesoParametroEmpresa
                    .Obtener(x => x.CodProceso == Constantes.CodigoProcesoBct &&
                                  x.CodEmpresa == request.CodEmpresa)
                    .ToListAsync();

                var toleranciaAlerta = parametros
                    .Where(x => x.CodParametro == Constantes.CodigoParametroToleranciaAlerta)
                    .Select(x => x.ValParametro)
                    .FirstOrDefault();

                var toleranciaCantidad = parametros
                    .Where(x => x.CodParametro == Constantes.CodigoParametroToleranciaCantidad)
                    .Select(x => x.ValParametro)
                    .FirstOrDefault();

                if (request.RegistroTotal == null)
                    return response;

                var cantidad = request.RegistroTotal.Cantidad;
                var tolerancia = Convert.ToInt32(toleranciaCantidad);

                // ================================================
                // NUEVA LÓGICA DE COLORES
                // ================================================
                // Verde → Cantidad <= tolerancia
                // Amarillo → margen leve sobre tolerancia (opcional)
                // Rojo → excede tolerancia (directo)
                // ================================================

                // Puedes ajustar el margen si quieres mantener "advertencia"
                double margenAmarillo = 0.05; // 5% por encima del límite (ajustable)
                double limiteAmarillo = tolerancia * (1 + margenAmarillo);

                if (cantidad <= tolerancia)
                {
                    luzVerde = true;
                    luzNaranja = false;
                    luzRoja = false;
                }
                else if (cantidad > tolerancia && cantidad <= limiteAmarillo)
                {
                    luzVerde = false;
                    luzNaranja = true;
                    luzRoja = false;
                }
                else
                {
                    luzVerde = false;
                    luzNaranja = false;
                    luzRoja = true;
                }

                // ================================================
                // Validar si ya existe una alerta activa y se venció
                // ================================================
                if (!string.IsNullOrEmpty(request.FechaAlerta) && !luzVerde)
                {
                    var fechaAlerta = DateTime.ParseExact(request.FechaAlerta, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);
                    var fechaActual = DateTime.Now;

                    if ((fechaActual - fechaAlerta).TotalMinutes > Convert.ToInt32(toleranciaAlerta))
                    {
                        luzNaranja = false;
                        luzVerde = false;
                        luzRoja = true;

                        // TODO: Enviar Alerta (correo, log, notificación, etc.)
                        envioNotificacion = 1;
                    }
                }

                response.Data = (luzVerde, luzNaranja, luzRoja, envioNotificacion);
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
            }

            return response;
        }
    }

    //public class ProcesarMonitorBCTHandler : IRequestHandler<ProcesarMonitorBCTCommand, ObtenerComunDTO<(bool, bool, bool, int)>>
    //{
    //	private readonly ISGPContexto _contexto;

    //	public ProcesarMonitorBCTHandler()
    //	{
    //		_contexto = new SGPContexto();
    //	}

    //	public async Task<ObtenerComunDTO<(bool, bool, bool, int)>> Handle(ProcesarMonitorBCTCommand request, CancellationToken cancellationToken)
    //	{
    //		var luzVerde = true;
    //		var luzNaranja = false;
    //		var luzRoja = false;
    //		var envioNotificacion = 0;

    //		if (request.CodEmpresa == "12")
    //		{
    //			request.CodEmpresa = "02";

    //           }

    //		var response = new ObtenerComunDTO<(bool, bool, bool, int)> { Ok = true };
    //		try
    //		{
    //			var semaforoColor = (ColorSemaforo)request.Color;
    //			var parametros = await _contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoBct &&
    //																						 x.CodEmpresa == request.CodEmpresa).ToListAsync();

    //			var toleranciaAlerta = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroToleranciaAlerta).Select(x => x.ValParametro).FirstOrDefault();
    //			var toleranciaCantidad = parametros.Where(x => x.CodParametro == Constantes.CodigoParametroToleranciaCantidad).Select(x => x.ValParametro).FirstOrDefault();

    //               if (request.RegistroTotal == null)
    //                   return response;

    //               if (request.RegistroTotal.Cantidad >= Convert.ToInt32(toleranciaCantidad) && semaforoColor != ColorSemaforo.ROJO)
    //               {
    //                   luzNaranja = true;
    //                   luzVerde = false;
    //               }

    //               if (semaforoColor == ColorSemaforo.ROJO)
    //               {
    //                   var diferencia = request.CantidadAnterior * 0.8;

    //                   if (request.RegistroTotal.Cantidad <= diferencia)
    //                   {
    //                       luzNaranja = true;
    //                       luzVerde = false;
    //                       luzRoja = false;
    //                       request.FechaAlerta = "";
    //                   }
    //                   else
    //                   {
    //                       luzNaranja = false;
    //                       luzVerde = false;
    //                       luzRoja = true;
    //                   }
    //               }

    //               if (!string.IsNullOrEmpty(request.FechaAlerta) && !luzVerde)
    //			{
    //				var fechaAlerta = DateTime.ParseExact(request.FechaAlerta, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);
    //				var fechaActual = DateTime.Now;

    //				if ((fechaActual - fechaAlerta).TotalMinutes > Convert.ToInt32(toleranciaAlerta))
    //				{
    //					luzNaranja = false;
    //					luzVerde = false;
    //					luzRoja = true;

    //					//TODO: Enviar Alerta
    //					envioNotificacion = 1;
    //				}
    //			}

    //			response.Data = (luzVerde, luzNaranja, luzRoja, envioNotificacion);
    //		}
    //		catch (Exception ex)
    //		{
    //			response.Ok = false;
    //			response.Mensaje = ex.Message;
    //		}

    //		return response;
    //	}
    //}
}
