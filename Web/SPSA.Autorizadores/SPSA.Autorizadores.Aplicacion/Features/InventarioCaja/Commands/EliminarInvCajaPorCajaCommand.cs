using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
    public class EliminarInvCajaPorCajaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public decimal NumCaja { get; set; }
    }

    public class EliminarInvCajaPorCajaHandler : IRequestHandler<EliminarInvCajaPorCajaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarInvCajaPorCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarInvCajaPorCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var invCajaTemp = _contexto.RepositorioInvCajas.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion
                                                                                && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal
                                                                                && x.NumCaja == request.NumCaja).ToList();
                _contexto.RepositorioInvCajas.EliminarRango(invCajaTemp);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Inventarios por num caja eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al eliminar el inventarios por num caja";
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }
    }
}
