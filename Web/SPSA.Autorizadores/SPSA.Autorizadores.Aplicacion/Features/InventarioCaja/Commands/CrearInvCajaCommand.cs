using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
    public class CrearInvCajaCommand : IRequest<RespuestaComunDTO>
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

    public class CrearInvCajaHandler : IRequestHandler<CrearInvCajaCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearInvCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearInvCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                bool existe = await _contexto.RepositorioInvCajas.Existe(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena
                                                                        && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal
                                                                        && x.NumCaja == request.NumCaja && x.CodActivo == request.CodActivo);
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Inventario cajas ya existe";
                    return respuesta;
                }

                var invCaja = _mapper.Map<InvCajas>(request);
                _contexto.RepositorioInvCajas.Agregar(invCaja);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Inventario cajas creado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al crear inventario  caja";
                _logger.Error(ex, "Ocurrió un error al crear inventario caja");
            }
            return respuesta;
        }
    }
}
