using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Caja.Command
{
    public class EliminarMaeCajasCommand : IRequest<RespuestaComunDTO>
    {
        public List<MaeCajaDTO> Cajas { get; set; }
    }

    public class EliminarMaestroCajasHandler : IRequestHandler<EliminarMaeCajasCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarMaestroCajasHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarMaeCajasCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            //try
            //{

            //    foreach (var cajaEliminar in request.Cajas)
            //    {
            //        var existeCaja = await _repositorioMaestroCaja.Obtener(cajaEliminar.CodEmpresa, cajaEliminar.CodCadena, cajaEliminar.CodRegion, cajaEliminar.CodZona, cajaEliminar.CodLocal, cajaEliminar.NumCaja);
            //        if (existeCaja == null)
            //        {
            //            respuesta.Ok = false;
            //            respuesta.Mensaje = "La caja no existe";
            //            return respuesta;
            //        }

            //        var caja = _mapper.Map<MaestroCaja>(cajaEliminar);
            //        caja.TipEstado = "E";
            //        await _repositorioMaestroCaja.Actualizar(caja);     
            //    }

            //    respuesta.Mensaje = "Cajas actualizado exitosamente.";

            //}
            //catch (Exception ex)
            //{
            //    respuesta.Ok = false;
            //    respuesta.Mensaje = "Ocurrió un error al crear local";
            //    _logger.Error(ex, "Ocurrió un error al crear local");
            //}
            return respuesta;
        }
    }
}
