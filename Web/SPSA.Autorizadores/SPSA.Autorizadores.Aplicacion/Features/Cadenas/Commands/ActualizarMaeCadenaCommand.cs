using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cadenas.Commands
{
    public class ActualizarMaeCadenaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string NomCadena { get; set; }
        public int CadNumero { get; set; }
    }

    public class ActualizarMaeCadenaHandler : IRequestHandler<ActualizarMaeCadenaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeCadenaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeCadenaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var cadena = await _contexto.RepositorioMaeCadena.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena).FirstOrDefaultAsync();
                if (cadena is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "La cadena no existe";
                    return respuesta;
                }

                _mapper.Map(request, cadena);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Cadena actualizado exitosamente.";
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
