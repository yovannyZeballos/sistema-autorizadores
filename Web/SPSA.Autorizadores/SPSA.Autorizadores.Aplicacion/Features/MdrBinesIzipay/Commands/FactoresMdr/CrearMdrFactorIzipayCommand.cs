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

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.FactoresMdr
{
    public class CrearMdrFactorIzipayCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string NumAno { get; set; }
        public string CodOperador { get; set; }
        public string CodClasificacion { get; set; }
        public decimal Factor { get; set; }
        public string UsuCreacion { get; set; }
    }

    public class CrearMdrFactorIzipayHandler : IRequestHandler<CrearMdrFactorIzipayCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMdrFactorIzipayHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMdrFactorIzipayCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                bool existe = await _contexto.RepositorioMdrFactorIzipay
                    .Existe(x =>
                        x.CodEmpresa == request.CodEmpresa &&
                        x.NumAno == request.NumAno &&
                        x.CodOperador == request.CodOperador &&
                        x.CodClasificacion == request.CodClasificacion);

                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Ya existe un factor MDR para esa empresa, año, operador y clasificación.";
                    return respuesta;
                }

                var entidad = _mapper.Map<Mdr_FactorIzipay>(request);
                entidad.FecCreacion = DateTime.Now;
                entidad.IndActivo = "S";

                _contexto.RepositorioMdrFactorIzipay.Agregar(entidad);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Factor creado exitosamente.";
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
