using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Commands;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Commands
{
    public class AsignarLocalCommand : IRequest<RespuestaComunDTO>
    {
        public string CodLocal { get; set; }
        public string CodCadena { get; set; }
    }

    public class AsignarLocalHandler : IRequestHandler<AsignarLocalCommand, RespuestaComunDTO>
    {
        private readonly IRepositorioLocal _repositorioLocal;

        public AsignarLocalHandler(IRepositorioLocal repositorioLocal)
        {
            _repositorioLocal = repositorioLocal;
        }

        public async Task<RespuestaComunDTO> Handle(AsignarLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();
            try
            {
                await _repositorioLocal.AsignarLocal(request.CodLocal, request.CodCadena);
                respuesta.Ok = true;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = $"Error al asignar local: {request.CodLocal}, cadena: {request.CodCadena} | {ex.Message}";
                respuesta.Ok = false;
            }

            return respuesta;
        }
    }
}
