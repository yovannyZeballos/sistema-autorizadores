using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Commands;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Data.Entity;


namespace SPSA.Autorizadores.Aplicacion.Features.Colaborador.Commands
{
    public class ActualizarMaeColaboradorCommand : IRequest<RespuestaComunDTO>
    {
        public string CoEmpr { get; set; }
        public string CodigoOfisis { get; set; }
        public string NoApelPate { get; set; }
        public string NoApelMate { get; set; }
        public string NoTrab { get; set; }
        public string TiDocuIden { get; set; }
        public string NuDocuIden { get; set; }
        public int FeIngrEmpr { get; set; }
        public int FeCeseTrab { get; set; }
        public string CoPlan { get; set; }
        public string DePlan { get; set; }
        public string TiSitu { get; set; }
        public string CoPuesTrab { get; set; }
        public string CoSede { get; set; }
        public string CoDepa { get; set; }
        public string DeDepa { get; set; }
        public string CoArea { get; set; }
        public string DeArea { get; set; }
        public string CoSecc { get; set; }
        public string DeSecc { get; set; }
        public string CoMotiSepa { get; set; }
        public string IndInterno { get; set; }
    }

    public class ActualizarMaeColaboradorHandler : IRequestHandler<ActualizarMaeColaboradorCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeColaboradorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeColaboradorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var colaborador = await _contexto.RepositorioMaeColaborador.Obtener(x => x.CoEmpr == request.CodigoOfisis).FirstOrDefaultAsync();
                if (colaborador is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Colaborador no existe";
                    return respuesta;
                }

                _mapper.Map(request, colaborador);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Colaborador actualizado exitosamente.";
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
