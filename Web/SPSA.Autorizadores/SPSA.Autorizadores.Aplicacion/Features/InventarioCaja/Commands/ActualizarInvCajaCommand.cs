using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
    public class ActualizarInvCajaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public decimal NumCaja { get; set; }
        public string CodActivo { get; set; }
        public string CodModelo { get; set; }
        public string CodSerie { get; set; }
        public string NumAdenda { get; set; }
        public DateTime? FecGarantia { get; set; }
        public string TipEstado { get; set; }
        public string TipProcesador { get; set; }
        public string Memoria { get; set; }
        public string DesSo { get; set; }
        public string VerSo { get; set; }
        public string CapDisco { get; set; }
        public string TipDisco { get; set; }
        public string DesPuertoBalanza { get; set; }
        public string TipoCaja { get; set; }
        public string Hostname { get; set; }
        public DateTime? FechaInicioLising { get; set; }
        public DateTime? FechaFinLising { get; set; }
    }

    public class ActualizarInvCajaHandler : IRequestHandler<ActualizarInvCajaCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarInvCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarInvCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var invCaja = await _contexto.RepositorioInvCajas.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena
                                                                        && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal
                                                                        && x.NumCaja == request.NumCaja && x.CodActivo == request.CodActivo).FirstOrDefaultAsync();

                if (invCaja is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Inventario caja no existe";
                    return respuesta;
                }

                _mapper.Map(request, invCaja);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Inventario caja actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, respuesta.Mensaje);
            }
            return respuesta;
        }
    }
}
