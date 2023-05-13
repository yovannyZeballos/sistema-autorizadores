using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Queries
{
    public class ListarCajasPorLocalQuery : IRequest<ListarCajasPorLocalDTO>
    {
        public string CodEmpresa { get; set; }
        public string CodFormato { get; set; }
        public string CodLocal { get; set; }
    }

    public class ListarCajasPorLocalHandler : IRequestHandler<ListarCajasPorLocalQuery, ListarCajasPorLocalDTO>
    {
        private readonly IRepositorioSovosCaja _repositorioSovosCaja;

        public ListarCajasPorLocalHandler(IRepositorioSovosCaja repositorioSovosCaja)
        {
            _repositorioSovosCaja = repositorioSovosCaja;
        }

        public async Task<ListarCajasPorLocalDTO> Handle(ListarCajasPorLocalQuery request, CancellationToken cancellationToken)
        {
            var cajas = new ListarCajasPorLocalDTO();

            var cajasDataTable = await _repositorioSovosCaja.ListarPorLocal(request.CodEmpresa, request.CodFormato, request.CodLocal);

            cajas.Columnas = new List<string>();
            foreach (DataColumn colum in cajasDataTable.Columns)
            {
                cajas.Columnas.Add(colum.ColumnName);
            }

            cajas.Cajas = cajasDataTable.AsEnumerable()
                     .Select(r => r.Table.Columns.Cast<DataColumn>()
                     .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                  ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
               ).ToList();

            return cajas;
        }
    }
}
