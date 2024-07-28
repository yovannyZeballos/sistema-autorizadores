using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Threading.Tasks;
using System.Threading;
using Serilog;

namespace SPSA.Autorizadores.Aplicacion.Features.Aperturas.Commands
{
    public class CrearAperturaCommand : IRequest<RespuestaComunDTO>
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
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
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

    public class CrearAperturaHandler : IRequestHandler<CrearAperturaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearAperturaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearAperturaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                bool existe = await _contexto.RepositorioApertura.Existe(x => x.CodLocalPMM == request.CodLocalPMM);
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Local apertura ya existe";
                    return respuesta;
                }

                Apertura apertura = _mapper.Map<Apertura>(request);

                _contexto.RepositorioApertura.Agregar(apertura);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Local apertura creado exitosamente.";
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
