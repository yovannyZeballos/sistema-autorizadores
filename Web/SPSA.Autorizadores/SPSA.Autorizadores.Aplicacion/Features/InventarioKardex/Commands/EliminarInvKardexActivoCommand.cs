using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Threading.Tasks;
using System.Threading;
using System;
using Serilog;
using System.Data.Entity;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class EliminarInvKardexActivoCommand : IRequest<RespuestaComunDTO>
    {
        public string Id { get; set; }
    }

    public class EliminarInvKardexActivoHandler : IRequestHandler<EliminarInvKardexActivoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarInvKardexActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarInvKardexActivoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var existe = await _contexto.RepositorioInvKardexActivo.Existe(x => x.Id == request.Id);
                if (!existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se ha encontrado ningún registro.";
                    return respuesta;
                }

                var invKardexTemp = await _contexto.RepositorioInvKardexActivo.Obtener(x => x.Id == request.Id).FirstOrDefaultAsync();
                _contexto.RepositorioInvKardexActivo.Eliminar(invKardexTemp);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Registro eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al momento de eliminar el registro.";
                _logger.Error(ex, respuesta.Mensaje);
            }

            return respuesta;
        }
    }
}
