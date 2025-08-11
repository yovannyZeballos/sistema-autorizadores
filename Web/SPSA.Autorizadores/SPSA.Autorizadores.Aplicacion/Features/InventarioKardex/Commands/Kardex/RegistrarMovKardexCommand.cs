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
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Kardex
{
    public class RegistrarMovKardexCommand : IRequest<RespuestaComunDTO>
    {
        // Datos obligatorios
        public string TipoMovimiento { get; set; }        // INGRESO | SALIDA
        public DateTime Fecha { get; set; }
        public string CodProducto { get; set; }
        public long SerieProductoId { get; set; }
        public string CodEmpresaDestino { get; set; }
        public string CodLocalDestino { get; set; }
        public string DesAreaGestion { get; set; }
        public string DesClaseStock { get; set; }
        public string DesEstadoStock { get; set; }

        // Opcionales
        public string NumGuia { get; set; }
        public string OrdenCompra { get; set; }
        public string NumTicket { get; set; }       
        public string Observaciones { get; set; }
        public int Cantidad { get; set; } = 1;
        public string Usuario { get; set; }
    }

    public class RegistrarMovKardexHandler : IRequestHandler<RegistrarMovKardexCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RegistrarMovKardexHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(RegistrarMovKardexCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                if (request.Cantidad <= 0)
                    return new RespuestaComunDTO { Ok = false, Mensaje = "La cantidad debe ser mayor a cero." };


                var serie = await _contexto.RepositorioMaeSerieProducto.Obtener(x => x.Id == request.SerieProductoId).FirstOrDefaultAsync();

                if (serie == null)
                    return new RespuestaComunDTO { Ok = false, Mensaje = "La serie no existe." };

                // Validación de stock para SALIDA
                if (string.Equals(request.TipoMovimiento, "SALIDA", StringComparison.OrdinalIgnoreCase))
                {
                    decimal cantidad = request.Cantidad; // stk_actual es NUMERIC
                    if (serie.StkActual < cantidad)
                    {
                        return new RespuestaComunDTO
                        {
                            Ok = false,
                            Mensaje = "No hay stock disponible para el producto-serie seleccionado."
                        };
                    }
                }

                var entidad = _mapper.Map<Mov_Kardex>(request);
                entidad.CodEmpresaOrigen = serie.CodEmpresa;
                entidad.CodLocalOrigen = serie.CodLocal;
                entidad.UsuCreacion = request.Usuario;
                entidad.FecCreacion = DateTime.Now;

                _contexto.RepositorioMovKardex.Agregar(entidad);


                serie.CodEmpresa = request.CodEmpresaDestino;
                serie.CodLocal = request.CodLocalDestino;
                //serie.CodActivo = string.IsNullOrEmpty(request.CodActivo) ? serie.CodActivo : request.CodActivo;
                serie.UsuModifica = request.Usuario;
                serie.FecModifica = DateTime.Now;

                if (string.Equals(request.TipoMovimiento, "INGRESO", StringComparison.OrdinalIgnoreCase))
                {
                    // Ingreso: incrementa stock
                    serie.StkActual += request.Cantidad;
                    if (serie.FecIngreso == null) serie.FecIngreso = request.Fecha;
                    // serie.IndEstado = "DISPONIBLE"; // opcional
                }
                else // SALIDA
                {
                    // Salida: decrementa stock
                    serie.StkActual -= request.Cantidad;
                    if (serie.StkActual < 0) serie.StkActual = 0; // seguridad extra
                    serie.FecSalida = request.Fecha;
                    serie.IndEstado = "EN_USO";
                }

                _contexto.RepositorioMaeSerieProducto.Actualizar(serie);

                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Movimiento registrado correctamente.";
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
