using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using SPSA.Autorizadores.Infraestructura.Repositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Cadenas.Commands
{
    public class CrearMaeCadenaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string NomCadena { get; set; }
        public int CadNumero { get; set; }
    }

    public class CrearMaeCadenaHandler : IRequestHandler<CrearMaeCadenaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeCadenaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeCadenaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                bool existe = await _contexto.RepositorioMaeCadena.Existe(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena);
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "La cadena ya existe";
                    return respuesta;
                }

                var cadena = _mapper.Map<Mae_Cadena>(request);
                _contexto.RepositorioMaeCadena.Agregar(cadena);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Cadena creado exitosamente.";
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
