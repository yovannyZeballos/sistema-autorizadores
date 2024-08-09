using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Colaborador.Commands
{
    public class CrearMaeColaboradorCommand : IRequest<RespuestaComunDTO>
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

    public class CrearMaeColaboradorHandler : IRequestHandler<CrearMaeColaboradorCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeColaboradorHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeColaboradorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                bool existe = await _contexto.RepositorioMaeColaborador.Existe(x => x.CoEmpr == request.CoEmpr && x.CodigoOfisis == request.CodigoOfisis);
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Colaborador ya existe";
                    return respuesta;
                }

                var colaborador = _mapper.Map<Mae_Colaborador>(request);
                _contexto.RepositorioMaeColaborador.Agregar(colaborador);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Colaborador creado exitosamente.";
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
