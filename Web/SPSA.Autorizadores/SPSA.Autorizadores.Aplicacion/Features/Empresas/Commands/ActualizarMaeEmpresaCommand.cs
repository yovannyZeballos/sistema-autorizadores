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
using System.Web;

namespace SPSA.Autorizadores.Aplicacion.Features.Empresas.Commands
{
    public class ActualizarMaeEmpresaCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string NomEmpresa { get; set; }
        public string CodSociedad { get; set; }
        public string CodEmpresaOfi { get; set; }
        public string Ruc { get; set; }
    }

    public class ActualizarMaestroEmpresaHandler : IRequestHandler<ActualizarMaeEmpresaCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaestroEmpresaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeEmpresaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var empresa = await _contexto.RepositorioMaeEmpresa.Obtener(x => x.CodEmpresa == request.CodEmpresa).FirstOrDefaultAsync();
                if (empresa is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "La empresa no existe";
                    return respuesta;
                }

                _mapper.Map(request, empresa);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Empresa actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al actualizar empresa";
                _logger.Error(ex, "Ocurrió un error al actualizar empresa");
            }
            return respuesta;
        }
    }
}
