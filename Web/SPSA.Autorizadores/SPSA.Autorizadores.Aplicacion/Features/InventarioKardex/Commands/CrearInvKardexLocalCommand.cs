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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class CrearInvKardexLocalCommand : IRequest<RespuestaComunDTO>
    {
        public int Id { get; set; }
        public string Sociedad { get; set; }
        public string NomLocal { get; set; }
    }

    public class CrearInvKardexLocalHandler : IRequestHandler<CrearInvKardexLocalCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearInvKardexLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearInvKardexLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var invKardexLocal = _mapper.Map<InvKardexLocal>(request);
                _contexto.RepositorioInvKardexLocal.Agregar(invKardexLocal);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Registro ingresado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al crear";
                _logger.Error(ex, "Ocurrió un error al crear KADEX LOCAL");
            }
            return respuesta;
        }
    }
}
