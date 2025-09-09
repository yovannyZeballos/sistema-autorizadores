using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{

    public class ListarColaboradoresCesadosQuery : IRequest<ColaboradoresCesadosDTO>
    {
    }

    //public class ListarColaboradoresCesadosHandler : IRequestHandler<ListarColaboradoresCesadosQuery, ColaboradoresCesadosDTO>
    //{
    //    private readonly IRepositorioAutorizadores _repositorioAutorizadores;

    //    public ListarColaboradoresCesadosHandler(IRepositorioAutorizadores repositorioAutorizadores)
    //    {
    //        _repositorioAutorizadores = repositorioAutorizadores;
    //    }

    //    public async Task<ColaboradoresCesadosDTO> Handle(ListarColaboradoresCesadosQuery request, CancellationToken cancellationToken)
    //    {
    //        var autorizadorEliminadoDTO = new ColaboradoresCesadosDTO();
    //        var autorizadoresDataTable = await _repositorioAutorizadores.ListarColaboradoresCesados();

    //        autorizadorEliminadoDTO.Columnas = new List<string>();
    //        foreach (DataColumn colum in autorizadoresDataTable.Columns)
    //        {
    //            autorizadorEliminadoDTO.Columnas.Add(colum.ColumnName);
    //        }

    //        autorizadorEliminadoDTO.Colaboradores = autorizadoresDataTable.AsEnumerable()
    //                 .Select(r => r.Table.Columns.Cast<DataColumn>()
    //                 .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
    //              ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
    //           ).ToList();



    //        return autorizadorEliminadoDTO;
    //    }
    //}
}
