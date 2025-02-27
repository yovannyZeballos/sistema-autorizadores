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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
    public class EliminarInvCajaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public decimal NumCaja { get; set; }
        public string CodActivo { get; set; }
    }

    public class EliminarInvCajaHandler : IRequestHandler<EliminarInvCajaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarInvCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarInvCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var existeInvCaja = await _contexto.RepositorioInvCajas.Existe(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena
                                                                        && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal
                                                                        && x.NumCaja == request.NumCaja && x.CodActivo == request.CodActivo);
                if (!existeInvCaja)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "El inventario caja no existe";
                    return respuesta;
                }

                var invCajaTemp = await _contexto.RepositorioInvCajas.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena
                                                                        && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal
                                                                        && x.NumCaja == request.NumCaja && x.CodActivo == request.CodActivo).FirstOrDefaultAsync();
                _contexto.RepositorioInvCajas.Eliminar(invCajaTemp);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Inventario caja elimindo exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al eliminar el inventario caja";
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }
    }
}
