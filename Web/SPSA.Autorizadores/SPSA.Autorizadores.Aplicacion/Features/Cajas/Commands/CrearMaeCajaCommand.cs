using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.Caja.Command
{
    public class CrearMaeCajaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public int NumCaja { get; set; }
        public string IpAddress { get; set; }
        public string TipOs { get; set; }
        public string TipEstado { get; set; }
    }

    public class CrearMaeCajaHandler : IRequestHandler<CrearMaeCajaCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeCajaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                bool existe = await _contexto.RepositorioMaeCaja.Existe(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal && x.NumCaja == request.NumCaja);
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Local ya existe";
                    return respuesta;
                }

                var caja = _mapper.Map<Mae_Caja>(request);
                _contexto.RepositorioMaeCaja.Agregar(caja);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Local creado exitosamente.";
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

