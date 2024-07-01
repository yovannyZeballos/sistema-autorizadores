using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Reportes.Queries
{
    public class ListarLocalesNotaCreditoQuery : IRequest<ListarComunDTO<Dictionary<string, object>>>
    {
        public string CodLocal { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class ListarLocalesNotaCreditoHandler : IRequestHandler<ListarLocalesNotaCreditoQuery, ListarComunDTO<Dictionary<string, object>>>
    {
        private readonly IRepositorioReportes _repositorioReportes;

        public ListarLocalesNotaCreditoHandler(IRepositorioReportes repositorioReportes)
        {
            _repositorioReportes = repositorioReportes;
        }

        public async Task<ListarComunDTO<Dictionary<string, object>>> Handle(ListarLocalesNotaCreditoQuery request, CancellationToken cancellationToken)
        {
            var response = new ListarComunDTO<Dictionary<string, object>> { Ok = true };

            try
            {
                response.Columnas = new List<string>();
                response.Data = new List<Dictionary<string, object>>();
                var dt = await _repositorioReportes.ListarLocalesNotaCredito(request.CodLocal, request.FechaInicio, request.FechaFin);
                foreach (DataColumn colum in dt.Columns)
                {
                    response.Columnas.Add(colum.ColumnName);
                }

                response.Data = dt.AsEnumerable()
                         .Select(r => r.Table.Columns.Cast<DataColumn>()
                         .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                      ).ToDictionary(z => z.Key.Replace(" ", "")
                                              .Replace(".", "")
                                              .Replace("á", "a")
                                              .Replace("é", "e")
                                              .Replace("í", "i")
                                              .Replace("ó", "o")
                                              .Replace("ú", "u"), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
                   ).ToList();
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
            }
            return response;
        }
    }
}
