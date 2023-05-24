using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Commands
{
    public class AsociarLocalPMMCommand : IRequest<RespuestaComunDTO>
    {
        public string CodCadenaCt2 { get; set; }
        public string CodLocalCt2 { get; set; }
        public string CodEmpresa { get; set; }
        public string CodSede { get; set; }

    }

    public class AsociarLocalPMMHandler : IRequestHandler<AsociarLocalPMMCommand, RespuestaComunDTO>
    {
        private readonly IRepositorioLocalOfiplan _repositorioLocalOfiplan;

        public AsociarLocalPMMHandler(IRepositorioLocalOfiplan repositorioLocalOfiplan)
        {
            _repositorioLocalOfiplan = repositorioLocalOfiplan;
        }

        public async Task<RespuestaComunDTO> Handle(AsociarLocalPMMCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();
            try
            {
                await _repositorioLocalOfiplan.Insertar(new LocalOfiplan(request.CodCadenaCt2, request.CodLocalCt2, request.CodLocalCt2, request.CodSede));
                respuesta.Ok = true;
                respuesta.Mensaje = "Local configurado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
            }
            return respuesta;
        }
    }
}
