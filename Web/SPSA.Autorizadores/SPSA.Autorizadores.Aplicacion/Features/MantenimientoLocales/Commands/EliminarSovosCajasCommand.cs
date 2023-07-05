using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands
{
    public class EliminarSovosCajasCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodFormato { get; set; }
        public string Cajas { get; set; }
    }

    public class EliminarSovosCajaHandler : IRequestHandler<EliminarSovosCajasCommand, RespuestaComunDTO>
    {
        readonly IRepositorioSovosCaja _repositorioSovosCaja;

        public EliminarSovosCajaHandler(IRepositorioSovosCaja repositorioSovosCaja)
        {
            _repositorioSovosCaja = repositorioSovosCaja;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarSovosCajasCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                await _repositorioSovosCaja.Eliminar(request.CodEmpresa, request.CodFormato, request.CodLocal, request.Cajas);
                respuesta.Ok = true;
                respuesta.Mensaje = "Cajas eliminadas exitosamente";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }
    }
}
