﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Reportes.Queries
{
    public class ListarValesRedimidosQuery : IRequest<ListarComunDTO<Dictionary<string, object>>>
    {
        public string CodLocal { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int StartRow { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Draw { get; set; } = 0;
    }

    public class ListarValesRedimidosHandler : IRequestHandler<ListarValesRedimidosQuery, ListarComunDTO<Dictionary<string, object>>>
    {
        private readonly IRepositorioReportes _repositorioReportes;

        public ListarValesRedimidosHandler(IRepositorioReportes repositorioReportes)
        {
            _repositorioReportes = repositorioReportes;
        }

        public async Task<ListarComunDTO<Dictionary<string, object>>> Handle(ListarValesRedimidosQuery request, CancellationToken cancellationToken)
        {
            var response = new ListarComunDTO<Dictionary<string, object>> { Ok = true };

            try
            {
                int endRow = request.StartRow + request.PageSize - 1;

                var dt = await _repositorioReportes.ListarValesRedimidosPaginadoAsync(
                    request.CodLocal,
                    request.FechaInicio,
                    request.FechaFin,
                    request.StartRow,
                    endRow
                );

                response.TotalRegistros = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["TOTAL_RECORDS"]) : 0;

                response.Columnas = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

                response.Data = dt.AsEnumerable()
                                .Select(r => r.Table.Columns.Cast<DataColumn>()
                                .Select(c => new KeyValuePair<string, object>(
                                    c.ColumnName,
                                    r[c.Ordinal] is DateTime dateTime ? dateTime.ToString("dd/MM/yyyy") : r[c.Ordinal]
                                ))
                                .ToDictionary(
                                    kvp => kvp.Key.Replace(" ", "").Replace(".", "").Replace("á", "a")
                                        .Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u"),
                                    kvp => kvp.Value
                                ))
                                .ToList();
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
