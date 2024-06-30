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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands
{
    public class CrearInvActivoCommand : IRequest<RespuestaComunDTO>
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
        public string Ip { get; set; }
        public string NomArea { get; set; }
        public string NumOc { get; set; }
        public string NumGuia { get; set; }
        public DateTime? FecSalida { get; set; }
        public int Antiguedad { get; set; }
        public string IndOperativo { get; set; }
        public string Observacion { get; set; }
        public string Garantia { get; set; }
        public DateTime? FecActualiza { get; set; }
    }

    public class CrearInvActivoHandler : IRequestHandler<CrearInvActivoCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearInvActivoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearInvActivoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                bool existe = await _contexto.RepositorioInventarioActivo.Existe(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena
                                                                        && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal
                                                                        && x.CodActivo == request.CodActivo && x.NomMarca == request.NomMarca && x.CodSerie == request.CodSerie);
                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Inventario activo ya existe";
                    return respuesta;
                }

                var invActivo = _mapper.Map<Inv_Activo>(request);
                _contexto.RepositorioInventarioActivo.Agregar(invActivo);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Inventario activo creado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al crear empresa";
                _logger.Error(ex, "Ocurrió un error al crear empresa");
            }
            return respuesta;
        }
    }
}
