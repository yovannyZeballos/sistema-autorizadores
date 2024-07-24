using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Caja.Command
{
    public class EliminarMaeCajasCommand : IRequest<RespuestaComunDTO>
    {
        public List<MaeCajaDTO> Cajas { get; set; }
    }

    public class EliminarMaeCajasHandler : IRequestHandler<EliminarMaeCajasCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaeCajasHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeCajasCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {

                foreach (var cajaEliminar in request.Cajas)
                {
                    var caja = await _contexto.RepositorioMaeCaja.Obtener(x => x.CodEmpresa == cajaEliminar.CodEmpresa && x.CodCadena == cajaEliminar.CodCadena && x.CodRegion == cajaEliminar.CodRegion 
                                                                    && x.CodZona == cajaEliminar.CodZona && x.CodLocal == cajaEliminar.CodLocal && x.NumCaja == cajaEliminar.NumCaja).FirstOrDefaultAsync();
                    if (caja is null)
                    {
                        respuesta.Ok = false;
                        respuesta.Mensaje = "Caja no existe";
                        return respuesta;
                    }

                    caja.TipEstado = "E";
                    _contexto.RepositorioMaeCaja.Actualizar(caja);
                    await _contexto.GuardarCambiosAsync();
                    respuesta.Mensaje = "Caja actualizado exitosamente.";
                }

                respuesta.Mensaje = "Cajas actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al crear local";
                _logger.Error(ex, "Ocurrió un error al crear local");
            }
            return respuesta;
        }
    }
}
