using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Entity;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Monitor.Queries
{
    public class ObtenerFechaNegocioQuery : IRequest<GenericResponseDTO<List<ParametrosMonitorBctDTO>>>
    {
    }

    public class ObtenerFechaNegocioHandler : IRequestHandler<ObtenerFechaNegocioQuery, GenericResponseDTO<List<ParametrosMonitorBctDTO>>>
    {
        private readonly ISGPContexto _contexto;

        public ObtenerFechaNegocioHandler()
        {
            _contexto = new SGPContexto();
        }

        public async Task<GenericResponseDTO<List<ParametrosMonitorBctDTO>>> Handle(ObtenerFechaNegocioQuery request, CancellationToken cancellationToken)
        {
            var parametrosMonitorBctDTO = new GenericResponseDTO<List<ParametrosMonitorBctDTO>> { Ok = true };
            try
            {
                var parametros = await _contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == Constantes.CodigoProcesoBct).ToListAsync();

                parametrosMonitorBctDTO.Data = parametros
                    .Where(x => x.CodParametro == Constantes.CodigoParametroFechaNegocioAlerta || x.CodParametro == Constantes.CodigoParametroConexionAlerta)
                    .GroupBy(x => x.CodEmpresa)
                    .Select(g => new ParametrosMonitorBctDTO
                    {
                        CodEmpresa = g.Key,
                        FechaNegocio = Convert.ToDateTime(g.FirstOrDefault(x => x.CodParametro == Constantes.CodigoParametroFechaNegocioAlerta)?.ValParametro).ToString("yyyy-MM-dd"),
                        HoraNegocio = Convert.ToDateTime(g.FirstOrDefault(x => x.CodParametro == Constantes.CodigoParametroFechaNegocioAlerta)?.ValParametro).ToString("HH:mm:ss"),
                        EstadoConexion = g.FirstOrDefault(x => x.CodParametro == Constantes.CodigoParametroConexionAlerta)?.ValParametro
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                parametrosMonitorBctDTO.Ok = false;
                parametrosMonitorBctDTO.Mensaje = ex.Message;
            }

            return parametrosMonitorBctDTO;

        }
    }
}
