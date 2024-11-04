using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class EliminarInvKardexLocalCommand : IRequest<RespuestaComunDTO>
    {
        public int Id { get; set; }
    }

    public class EliminarInvKardexLocalHandler : IRequestHandler<EliminarInvKardexLocalCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarInvKardexLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarInvKardexLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var existe = await _contexto.RepositorioInvKardexLocal.Existe(x => x.Id == request.Id);
                if (!existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se ha encontrado ningún registro.";
                    return respuesta;
                }

                var invKardexTemp = await _contexto.RepositorioInvKardexLocal.Obtener(x => x.Id == request.Id).FirstOrDefaultAsync();
                _contexto.RepositorioInvKardexLocal.Eliminar(invKardexTemp);
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
