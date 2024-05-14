using AutoMapper;
using MediatR;
using Npgsql;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Commands
{
    public class ActualizarMaeLocalCommand : IRequest<RespuestaComunDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public string NomLocal { get; set; }
        public string TipEstado { get; set; }
        public string CodLocalPMM { get; set; }
        public string CodLocalOfiplan { get; set; }
        public string NomLocalOfiplan { get; set; }
        public string CodLocalSunat { get; set; }

        public string CodRegionAnterior { get; set; }
        public string CodZonaAnterior { get; set; }
    }

    public class ActualizarMaeLocalHandler : IRequestHandler<ActualizarMaeLocalCommand, RespuestaComunDTO>
    {
        private readonly IBCTContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ActualizarMaeLocalHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        //public async Task<RespuestaComunDTO> Handle(ActualizarMaeLocalCommand request, CancellationToken cancellationToken)
        //{
        //    var respuesta = new RespuestaComunDTO { Ok = true };
        //    try
        //    {
        //        var local = await _contexto.RepositorioMaeLocal.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegionAnterior && x.CodZona == request.CodZonaAnterior && x.CodLocal == request.CodLocal).FirstOrDefaultAsync();
        //        if (local is null)
        //        {
        //            respuesta.Ok = false;
        //            respuesta.Mensaje = "Local no existe";
        //            return respuesta;
        //        }

        //        _mapper.Map(request, local);
        //        await _contexto.GuardarCambiosAsync();
        //        respuesta.Mensaje = "Local actualizado exitosamente.";
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Ok = false;
        //        respuesta.Mensaje = ex.Message;
        //        _logger.Error(ex, respuesta.Mensaje);
        //    }
        //    return respuesta;
        //}

        public async Task<RespuestaComunDTO> Handle(ActualizarMaeLocalCommand request, CancellationToken cancellationToken)
        {
            if (request.NomLocal is null)
            {
                request.NomLocal = string.Empty;
            }


            if (request.NomLocalOfiplan is null)
            {
                request.NomLocalOfiplan = string.Empty;
            }

            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var sql = @"UPDATE ""SGP"".""MAE_LOCAL""
                                SET ""COD_REGION""=@nuevaRegion, ""COD_ZONA""=@nuevaZona, ""NOM_LOCAL""=@nomLocal, ""TIP_ESTADO""=@tipEstado, ""COD_LOCAL_PMM""=@codLocalPMM, 
                                    ""COD_LOCAL_OFIPLAN""=@codLocalOfiplan, ""NOM_LOCAL_OFIPLAN""=@nomLocalOfiplan, ""COD_LOCAL_SUNAT""=@codLocalSunat
                                WHERE ""COD_EMPRESA""=@codEmpresa AND ""COD_CADENA""=@codCadena 
                                    AND ""COD_REGION""=@codRegionAnterior  AND ""COD_ZONA""=@codZonaAnterior 
                                    AND ""COD_LOCAL""=@codLocal;";
                
                var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@nuevaRegion", request.CodRegion),
            new NpgsqlParameter("@nuevaZona", request.CodZona),
            new NpgsqlParameter("@codEmpresa", request.CodEmpresa),
            new NpgsqlParameter("@codCadena", request.CodCadena),
            new NpgsqlParameter("@codLocal", request.CodLocal),
            new NpgsqlParameter("@nomLocal", request.NomLocal),
            new NpgsqlParameter("@tipEstado", request.TipEstado),
            new NpgsqlParameter("@codLocalPMM", Convert.ToInt32(request.CodLocalPMM)),
            new NpgsqlParameter("@codLocalOfiplan", request.CodLocalOfiplan),
            new NpgsqlParameter("@nomLocalOfiplan", request.NomLocalOfiplan),
            new NpgsqlParameter("@codLocalSunat", request.CodLocalSunat),
            new NpgsqlParameter("@codRegionAnterior", request.CodRegionAnterior),
            new NpgsqlParameter("@codZonaAnterior", request.CodZonaAnterior)
        };

                var affectedRows = await _contexto.RepositorioMaeLocal.ExecuteSqlCommandAsync(sql, parameters.ToArray());

                if (affectedRows > 0)
                {
                    respuesta.Mensaje = "Local actualizado exitosamente.";
                }
                else
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = "No se encontraron registros para actualizar.";
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = ex.Message;
                _logger.Error(ex, "Error al actualizar el local: " + ex.Message);
            }

            return respuesta;
        }

    }
}
