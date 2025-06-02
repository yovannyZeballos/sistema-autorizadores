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

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.Bines
{
    public class CrearMdrBinesIzipayCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string NumAno { get; set; }
        public string NumBin6 { get; set; }
        public string NomTarjeta { get; set; }
        public string NumBin8 { get; set; }
        public string BancoEmisor { get; set; }
        public string Tipo { get; set; }
        public decimal FactorMdr { get; set; }
        public string CodOperador { get; set; }
    }

    public class CrearMdrBinesIzipayHandler : IRequestHandler<CrearMdrBinesIzipayCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CrearMdrBinesIzipayHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(CrearMdrBinesIzipayCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                // Chequear si ya existe un registro con la misma clave (CodEmpresa + NumAno + NumBin6)
                bool existe = await _contexto.RepositorioMdrBinesIzipay
                    .Existe(x =>
                        x.CodEmpresa == request.CodEmpresa &&
                        x.NumAno == request.NumAno &&
                        x.NumBin6 == request.NumBin6);

                if (existe)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "Ya existe un registro de BIN para esa empresa, año y BIN6.";
                    return respuesta;
                }

                // Mapear propiedades de comando a entidad
                var entidad = _mapper.Map<Mdr_BinesIzipay>(request);

                // Agregar y guardar
                _contexto.RepositorioMdrBinesIzipay.Agregar(entidad);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "BIN creado exitosamente.";
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
