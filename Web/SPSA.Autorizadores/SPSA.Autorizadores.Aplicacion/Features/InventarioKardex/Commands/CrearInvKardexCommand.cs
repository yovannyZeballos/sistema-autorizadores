using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands
{
    public class CrearInvKardexCommand : IRequest<RespuestaComunDTO>
    {
        public string Kardex { get; set; }
        public DateTime Fecha { get; set; }
        public string Guia { get; set; }
        public string ActivoId { get; set; }
        public string Serie { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Tk { get; set; }
        public int Cantidad { get; set; }
        public string TipoStock { get; set; }
        public string Oc { get; set; }
        public string Sociedad { get; set; }
    }

    public class CrearInvKardexHandler : IRequestHandler<CrearInvKardexCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearInvKardexHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearInvKardexCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                //bool existe = await _contexto.RepositorioInventarioActivo.Existe(x => x.CodEmpresa == request.CodEmpresa);
                //if (existe)
                //{
                //    respuesta.Ok = false;
                //    respuesta.Mensaje = "Inventario kardex ya existe";
                //    return respuesta;
                //}

                var invKardex = _mapper.Map<InvKardex>(request);
                _contexto.RepositorioInvKardex.Agregar(invKardex);
                await _contexto.GuardarCambiosAsync();
                respuesta.Mensaje = "Registro ingresado exitosamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Ocurrió un error al crear inv kardex";
                _logger.Error(ex, "Ocurrió un error al crear inv kardex");
            }
            return respuesta;
        }
    }
}
