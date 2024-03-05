using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Zonas.Commands;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using SPSA.Autorizadores.Infraestructura.Repositorio;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;


namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Commands
{
    public class ActualizarMaeLocalCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public string NomLocal { get; set; }
        public string TipEstado { get; set; }
        public string CodLocalPMM { get; set; }
        public string CodLocalOfiplan { get; set; }
        public string NomLocalOfiplan { get; set; }
        public string CodLocalSunat { get; set; }
    }

    public class ActualizarMaeLocalHandler : IRequestHandler<ActualizarMaeLocalCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var local = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal).FirstOrDefaultAsync();
                if (local is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "La Zona no existe";
                    return respuesta;
                }

                _mapper.Map(request, local);
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
