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

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Commands
{
    public class CrearMaeEmpresaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string NomEmpresa { get; set; }
        public string CodSociedad { get; set; }
        public string CodEmpresaOfi { get; set; }
        public string Ruc { get; set; }
    }

    public class CrearMaestroEmpresaHandler : IRequestHandler<CrearMaeEmpresaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaestroEmpresaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeEmpresaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                bool existe = await _contexto.RepositorioMaeEmpresa.Existe(x => x.CodEmpresa == request.CodEmpresa);
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "La empresa ya existe";
                    return respuesta;
                }

                var empresa = _mapper.Map<Mae_Empresa>(request);
                _contexto.RepositorioMaeEmpresa.Agregar(empresa);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Empresa creado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al crear empresa";
                _logger.Error(ex, "Ocurrió un error al crear empresa");
            }
            return respuesta;
        }
    }
}
