using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands
{
    public class EliminarInvActivoCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public string CodActivo { get; set; }
        public string CodModelo { get; set; }
        public string CodSerie { get; set; }
        public string NomMarca { get; set; }
    }

    public class EliminarInvActivoHandler : IRequestHandler<EliminarInvActivoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EliminarInvActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(EliminarInvActivoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var invActivo = await _contexto.RepositorioInventarioActivo.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena
                                                                        && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal
                                                                        && x.CodActivo == request.CodActivo && x.NomMarca == request.NomMarca && x.CodSerie == request.CodSerie).FirstOrDefaultAsync();

                if (invActivo is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Inventario activo no existe";
                    return respuesta;
                }

                _contexto.RepositorioInventarioActivo.Eliminar(invActivo);
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
