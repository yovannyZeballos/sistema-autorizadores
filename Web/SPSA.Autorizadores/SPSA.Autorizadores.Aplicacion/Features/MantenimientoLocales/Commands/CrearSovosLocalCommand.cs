using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Monitor.Commands;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands
{
    public class CrearSovosLocalCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string CodFormato { get; set; }
        public string NomLocal { get; set; }
        public string Ip { get; set; }
        public string IpMascara { get; set; }
        public string SO { get; set; }
        public decimal? Grupo { get; set; }
        public string Estado { get; set; }
        public string TipoLocal { get; set; }
        public string IndFactura { get; set; }
        public string CodigoSunat { get; set; }
    }

    public class CrearSovosLocalHandler : IRequestHandler<CrearSovosLocalCommand, RespuestaComunDTO>
    {
        readonly IRepositorioSovosLocal _repositorioSovosLocal;

        public CrearSovosLocalHandler(IRepositorioSovosLocal repositorioSovosLocal)
        {
            _repositorioSovosLocal = repositorioSovosLocal;
        }

        public async Task<RespuestaComunDTO> Handle(CrearSovosLocalCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO();

            try
            {
                var local = new SovosLocal(request.CodEmpresa, request.CodLocal, request.CodFormato, request.NomLocal, request.Ip,
                    request.IpMascara, request.SO, request.Grupo, request.Estado, request.TipoLocal, request.IndFactura, request.CodigoSunat);

                await _repositorioSovosLocal.Crear(local);
                respuesta.Ok = true;
                respuesta.Mensaje = "Local guardado exitosamente";
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
