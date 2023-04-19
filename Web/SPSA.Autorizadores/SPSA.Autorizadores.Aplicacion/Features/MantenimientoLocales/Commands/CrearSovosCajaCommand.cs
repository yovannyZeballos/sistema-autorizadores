using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoCajaes.Commands
{
    public class CrearSovosCajaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodFormato { get; set; }
        public decimal NumeroCaja { get; set; }
        public string Ip { get; set; }
        public string So { get; set; }

    }

    public class CrearSovosCajaHandler : IRequestHandler<CrearSovosCajaCommand, RespuestaComunDTO>
    {
        readonly IRepositorioSovosCaja _repositorioSovosCaja;

        public CrearSovosCajaHandler(IRepositorioSovosCaja repositorioSovosCaja)
        {
            _repositorioSovosCaja = repositorioSovosCaja;
        }

        public async Task<RespuestaComunDTO> Handle(CrearSovosCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                var caja = new SovosCaja(request.CodEmpresa, request.CodLocal, request.CodFormato, request.NumeroCaja, request.Ip, request.So);

                await _repositorioSovosCaja.Crear(caja);
                respuesta.Ok = true;
                respuesta.Mensaje = "Caja guardada exitosamente";
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
