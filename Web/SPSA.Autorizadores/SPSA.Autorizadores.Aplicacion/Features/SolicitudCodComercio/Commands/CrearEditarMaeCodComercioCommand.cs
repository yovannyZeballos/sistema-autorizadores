using System;
using AutoMapper;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.SolicitudCodComercio.Commands
{
    public class CrearEditarMaeCodComercioCommand : IRequest<RespuestaComunDTO>
    {
        public decimal NroSolicitud { get; set; }
        public int CodLocalAlterno { get; set; }
        public string CodComercio { get; set; }
        public string NomCanalVta { get; set; }
        public string DesOperador { get; set; }
        public string IndActiva { get; set; }
        public string UsuCreacion { get; set; }
        public long? NomProcesador { get; set; }
        public bool EsEdicion { get; set; }
    }

    public class CrearEditarMaeCodComercioHandler : IRequestHandler<CrearEditarMaeCodComercioCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearEditarMaeCodComercioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearEditarMaeCodComercioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                if (await ExisteComercioActivoEnOtroLocal(request))
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "El código comercio ya se encuentra en uso en otro local.";
                    return respuesta;
                }

                if (request.EsEdicion)
                {
                    var actualizado = await ActualizarCodComercio(request, respuesta);
                    if (!actualizado) return respuesta;
                }
                else
                {
                    CrearNuevoCodComercio(request);
                    respuesta.Mensaje = "Código comercio creado exitosamente.";
                }

                await _contexto.GuardarCambiosAsync();
            }
            catch (Exception ex)
            {
                var mensaje = ContainsConstraintViolation(ex, "pk_mae_local_comercio")
                ? "El código comercio ya se encuentra en uso en este local."
                : "Ocurrió un error.";

                _logger.Error(ex, "Error al crear o actualizar código comercio.");
                return new RespuestaComunDTO { Ok = false, Mensaje = mensaje };
            }

            return respuesta;
        }

        private async Task<bool> ExisteComercioActivoEnOtroLocal(CrearEditarMaeCodComercioCommand request)
        {
            if (request.IndActiva == "S")
            {
                return await _contexto.RepositorioMaeCodComercio.Existe(x =>
                    x.CodComercio == request.CodComercio &&
                    x.IndActiva == "S" &&
                    !(x.NroSolicitud == request.NroSolicitud && x.CodLocalAlterno == request.CodLocalAlterno)
                );
            }
            return false;
        }

        private async Task<bool> ActualizarCodComercio(CrearEditarMaeCodComercioCommand request, RespuestaComunDTO respuesta)
        {
            var comercio = await _contexto.RepositorioMaeCodComercio.Obtener(x =>
                x.NroSolicitud == request.NroSolicitud &&
                x.CodLocalAlterno == request.CodLocalAlterno &&
                x.CodComercio == request.CodComercio
            ).FirstOrDefaultAsync();

            if (comercio == null)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "El código comercio no fue encontrado.";
                return false;
            }

            comercio.NomCanalVta = request.NomCanalVta;
            comercio.DesOperador = request.DesOperador;
            comercio.IndActiva = request.IndActiva;
            comercio.FecModifica = DateTime.UtcNow;
            comercio.UsuModifica = request.UsuCreacion;

            respuesta.Mensaje = "Código comercio actualizado correctamente.";
            return true;
        }

        private void CrearNuevoCodComercio(CrearEditarMaeCodComercioCommand request)
        {
            var nuevoComercio = _mapper.Map<Mae_CodComercio>(request);
            nuevoComercio.FecCreacion = DateTime.UtcNow;
            _contexto.RepositorioMaeCodComercio.Agregar(nuevoComercio);
        }

        private bool ContainsConstraintViolation(Exception ex, string constraintName)
        {
            while (ex != null)
            {
                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.Contains(constraintName))
                {
                    return true;
                }
                ex = ex.InnerException;
            }
            return false;
        }

    }
}
