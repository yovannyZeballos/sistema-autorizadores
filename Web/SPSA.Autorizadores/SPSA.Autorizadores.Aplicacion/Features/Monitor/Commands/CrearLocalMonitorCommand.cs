using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands
{
    public class CrearLocalMonitorCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public DateTime FechaProceso { get; set; }
        public DateTime FechaCierre { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }
        public string Formato { get; set; }
    }
    public class CrearLocalMonitorHandler : IRequestHandler<CrearLocalMonitorCommand, RespuestaComunDTO>
    {
        private readonly IRepositorioMonitorReporte _repositorioLocalMonitor;

        public CrearLocalMonitorHandler(IRepositorioMonitorReporte repositorioLocalMonitor)
        {
            _repositorioLocalMonitor = repositorioLocalMonitor;
        }

        public async Task<RespuestaComunDTO> Handle(CrearLocalMonitorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                var localMonitor = new MonitorReporte(request.CodEmpresa, request.CodLocal, request.FechaProceso,
                    request.FechaCierre, request.HoraInicio, request.HoraFin, request.Estado, request.Observacion, request.Formato);

                await _repositorioLocalMonitor.Crear(localMonitor);
                respuesta.Ok = true;
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
