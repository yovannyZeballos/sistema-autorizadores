using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Caja.Command
{
    public class ActualizarMaeCajaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public int NumCaja { get; set; }
        public string IpAddress { get; set; }
        public string TipOs { get; set; }
        public string TipEstado { get; set; }
    }

    public class ActualizarMaeCajaHandler : IRequestHandler<ActualizarMaeCajaCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var caja = await _contexto.RepositorioMaeCaja.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.NumCaja == request.NumCaja).FirstOrDefaultAsync();
                if (caja is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Caja no existe";
                    return respuesta;
                }

                _mapper.Map(request, caja);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Caja actualizado exitosamente.";
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
