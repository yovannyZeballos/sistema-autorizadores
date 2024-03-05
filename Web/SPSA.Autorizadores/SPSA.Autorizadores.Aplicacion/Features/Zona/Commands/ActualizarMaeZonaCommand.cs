using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Commands;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Zona.Commands
{
    public class ActualizarMaeZonaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string NomZona { get; set; }
        public string CodCordina { get; set; }
    }

    public class ActualizarMaeZonaHandler : IRequestHandler<ActualizarMaeZonaCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeZonaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeZonaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var zona = await _contexto.RepositorioMaeZona.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona).FirstOrDefaultAsync();
                if (zona is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "La Zona no existe";
                    return respuesta;
                }

                _mapper.Map(request, zona);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Zona actualizado exitosamente.";
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
