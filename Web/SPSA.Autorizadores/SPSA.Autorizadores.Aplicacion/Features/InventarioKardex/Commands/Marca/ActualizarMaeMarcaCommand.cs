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

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands.Marca
{
    public class ActualizarMaeMarcaCommand : IRequest<RespuestaComunDTO>
    {
        public int Id { get; set; }
        public string NomMarca { get; set; }
        public string IndActivo { get; set; }
        public string UsuModifica { get; set; }
    }

    public class ActualizarMaeMarcaHandler : IRequestHandler<ActualizarMaeMarcaCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeMarcaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeMarcaCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };

            try
            {
                var marca = await _contexto.RepositorioMaeMarca
                    .Obtener(x =>
                        x.Id == request.Id)
                    .FirstOrDefaultAsync(); ;

                if (marca is null)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encuentra marca con este código.";
                    return respuesta;
                }

                marca.NomMarca = request.NomMarca.ToUpper();
                marca.IndActivo = request.IndActivo;
                marca.UsuModifica = request.UsuModifica;
                marca.FecModifica = DateTime.Now;
                

                _contexto.RepositorioMaeMarca.Actualizar(marca);
                await _contexto.GuardarCambiosAsync();

                respuesta.Mensaje = "Marca actualizado exitosamente.";
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
