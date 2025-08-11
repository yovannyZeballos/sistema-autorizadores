using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.SerieProducto
{
    public class CrearMaeSerieProductoCommand : IRequest<RespuestaComunDTO>
    {
        public string CodProducto { get; set; } = null;
        public string NumSerie { get; set; } = null;
        public string IndEstado { get; set; }

        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public int StkActual { get; set; } = 0;

        public DateTime? FecIngreso { get; set; }
        public DateTime? FecSalida { get; set; }

        public DateTime FecCreacion { get; set; }
        public string UsuCreacion { get; set; }
        public DateTime? FecModifica { get; set; }
        public string UsuModifica { get; set; }
    }

    public class CrearMaeSerieProductoHandler : IRequestHandler<CrearMaeSerieProductoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMaeSerieProductoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMaeSerieProductoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var entidad = _mapper.Map<Mae_SerieProducto>(request);
                entidad.FecCreacion = DateTime.Now;

                _contexto.RepositorioMaeSerieProducto.Agregar(entidad);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Serie agregado exitosamente.";
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
