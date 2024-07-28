
using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.Aperturas.Commands
{
    public class ActualizarAperturaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodLocalPMM { get; set; }
        public string NomLocalPMM { get; set; }
        public string CodLocalSAP { get; set; }
        public string NomLocalSAP { get; set; }
        public string CodLocalSAPNew { get; set; }
        public string CodLocalOfiplan { get; set; }
        public string NomLocalOfiplan { get; set; }
        public string Administrador { get; set; }
        public string NumTelefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Ubigeo { get; set; }
        public string CodComercio { get; set; }
        public string CodCentroCosto { get; set; }
        public DateTime FecApertura { get; set; }
        public string TipEstado { get; set; }
        public string UsuModifica { get; set; }
        public DateTime FecModifica { get; set; }
        public string CodEAN { get; set; }
        public string CodSUNAT { get; set; }
        public string NumGuias { get; set; }
        public string CentroDistribu { get; set; }
        public string TdaEspejo { get; set; }
        public string Mt2Sala { get; set; }
        public string Spaceman { get; set; }
        public string ZonaPricing { get; set; }
        public string Geolocalizacion { get; set; }
        public DateTime FecCierre { get; set; }
    }

    public class ActualizarAperturaHandler : IRequestHandler<ActualizarAperturaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarAperturaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarAperturaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var apertura = await _contexto.RepositorioApertura.Obtener(x => x.CodLocalPMM == request.CodLocalPMM).FirstOrDefaultAsync();
                if (apertura is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Local apertura no existe";
                    return respuesta;
                }

                _mapper.Map(request, apertura);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Local apertura actualizado exitosamente.";
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
