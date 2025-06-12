using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.MdrBinesIzipay.Commands.FactoresMdr
{
    public class ActualizarMdrFactorIzipayCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string NumAno { get; set; }
        public string CodOperador { get; set; }
        public string CodClasificacion { get; set; }
        public decimal Factor { get; set; }
        public string UsuModifica { get; set; }
        public string IndActivo { get; set; }
    }

    public class ActualizarMdrFactorIzipayHandler : IRequestHandler<ActualizarMdrFactorIzipayCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMdrFactorIzipayHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMdrFactorIzipayCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var factor = await _contexto.RepositorioMdrFactorIzipay
                    .Obtener(x =>
                        x.CodEmpresa == request.CodEmpresa &&
                        x.NumAno == request.NumAno &&
                        x.CodOperador == request.CodOperador &&
                        x.CodClasificacion == request.CodClasificacion)
                    .FirstOrDefaultAsync(); ;

                if (factor is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No existe un factor MDR para esa empresa, año, operador y clasificación.";
                    return respuesta;
                }

                factor.Factor = request.Factor;
                factor.UsuModifica = request.UsuModifica;
                factor.FecModifica = DateTime.Now;
                factor.IndActivo = request.IndActivo;

                _contexto.RepositorioMdrFactorIzipay.Actualizar(factor);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Factor actualizado exitosamente.";
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
