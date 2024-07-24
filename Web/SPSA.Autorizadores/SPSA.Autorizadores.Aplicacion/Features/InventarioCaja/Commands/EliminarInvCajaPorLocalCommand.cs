using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
    public class EliminarInvCajaPorLocalCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
    }

    public class EliminarInvCajaPorLocalHandler : IRequestHandler<EliminarInvCajaPorLocalCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarInvCajaPorLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarInvCajaPorLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var invCajaTemp = _contexto.RepositorioInvCajas.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion 
                                                                                && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal).ToList();
                _contexto.RepositorioInvCajas.EliminarRango(invCajaTemp);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Inventarios de cajas por local elimindo exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al eliminar el inventarios caja por local";
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }
    }
}
