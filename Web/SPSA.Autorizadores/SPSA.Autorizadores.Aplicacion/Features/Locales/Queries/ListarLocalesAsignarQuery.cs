using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Locales.Queries
{

    public class ListarLocalesAsignarQuery : IRequest<LocalesAsignadosDTO>
    {
    }

    public class ListarLocalesAsignarHandler : IRequestHandler<ListarLocalesAsignarQuery, LocalesAsignadosDTO>
    {
        private readonly IRepositorioLocal _repositorioLocal;

        public ListarLocalesAsignarHandler(IRepositorioLocal repositorioLocal)
        {
            _repositorioLocal = repositorioLocal;
        }

        public async Task<LocalesAsignadosDTO> Handle(ListarLocalesAsignarQuery request, CancellationToken cancellationToken)
        {
            var localesAsignadosDTO = new LocalesAsignadosDTO();
            var autorizadoresDataTable = await _repositorioLocal.ListaLocalesAsignar();

            localesAsignadosDTO.Columnas = new List<string>();
            foreach (DataColumn colum in autorizadoresDataTable.Columns)
            {
                localesAsignadosDTO.Columnas.Add(colum.ColumnName);
            }

            localesAsignadosDTO.Locales = autorizadoresDataTable.AsEnumerable()
                     .Select(r => r.Table.Columns.Cast<DataColumn>()
                     .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                  ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
               ).ToList();



            return localesAsignadosDTO;
        }
    }
}
