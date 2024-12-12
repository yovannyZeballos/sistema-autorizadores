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

namespace SPSA.Autorizadores.Aplicacion.Features.Horarios.Commands
{
    public class CrearMaeHorarioCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public int NumDia { get; set; }
        public string CodDia { get; set; }
        public string HorOpen { get; set; }
        public string HorClose { get; set; }
        public string MinLmt { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
    }

    public class CrearMaeHorarioHandler : IRequestHandler<CrearMaeHorarioCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeHorarioHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeHorarioCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                bool existe = await _contexto.RepositorioMaeHorario.Existe(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal && x.NumDia == request.NumDia);
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Ya existe un horario para ese día";
                    return respuesta;
                }

                var horario = _mapper.Map<Mae_Horario>(request);
                _contexto.RepositorioMaeHorario.Agregar(horario);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Horario creado exitosamente.";
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
