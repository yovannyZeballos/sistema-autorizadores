using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.PeriodosMdr
{
    public class CrearMdrPeriodoCommand : IRequest<RespuestaComunDTO>
    {
        public string DesPeriodo { get; set; }
        public string IndActivo { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class CrearMdrPeriodoHandler : IRequestHandler<CrearMdrPeriodoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMdrPeriodoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMdrPeriodoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var entidad = _mapper.Map<Mdr_Periodo>(request);
                entidad.FecCreacion = DateTime.Now;

                _contexto.RepositorioMdrPeriodo.Agregar(entidad);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Periodo creado exitosamente.";
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
