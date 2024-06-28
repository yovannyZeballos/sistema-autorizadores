using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands
{
    public class ActualizarInvActivoCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public string CodActivo { get; set; }
        public string CodModelo { get; set; }
        public string CodSerie { get; set; }
        public string NomMarca { get; set; }
        public string Ip { get; set; }
        public string NomArea { get; set; }
        public string NumOc { get; set; }
        public string NumGuia { get; set; }
        public DateTime? FecSalida { get; set; }
        public int Antiguedad { get; set; }
        public string IndOperativo { get; set; }
        public string Observacion { get; set; }
        public string Garantia { get; set; }
        public DateTime? FecActualiza { get; set; }
    }

    public class ActualizarInvActivoHandler : IRequestHandler<ActualizarInvActivoCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarInvActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarInvActivoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var invActivo = await _contexto.RepositorioInventarioActivo.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena
                                                                        && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal
                                                                        && x.CodActivo == request.CodActivo && x.NomMarca == request.NomMarca && x.CodSerie == request.CodSerie).FirstOrDefaultAsync();

                if (invActivo is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Inventario activo no existe";
                    return respuesta;
                }

                _mapper.Map(request, invActivo);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Inventario activo actualizado exitosamente.";
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
