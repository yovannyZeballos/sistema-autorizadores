using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Puestos.Queries
{
    public class ListarPuestosQuery : IRequest<PuestoDTO>
    {
        public string CodEmpresa { get; set; }
    }

    public class ListarPuestosHandler : IRequestHandler<ListarPuestosQuery, PuestoDTO>
    {
        private readonly IRepositorioPuesto _repositorioPuesto;

        public ListarPuestosHandler(IRepositorioPuesto repositorioPuesto)
        {
            _repositorioPuesto = repositorioPuesto;
        }

        public async Task<PuestoDTO> Handle(ListarPuestosQuery request, CancellationToken cancellationToken)
        {
            var puestoDTO = new PuestoDTO();
            var puestosDataTable = await _repositorioPuesto.Listar(request.CodEmpresa);

            puestoDTO.Columnas = new List<string>();
            foreach (DataColumn colum in puestosDataTable.Columns)
            {
                puestoDTO.Columnas.Add(colum.ColumnName);
            }

            puestoDTO.Puestos = puestosDataTable.AsEnumerable()
                     .Select(r => r.Table.Columns.Cast<DataColumn>()
                     .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                  ).ToDictionary(z => z.Key.Replace(" ", "").Replace(".", ""), z => z.Value.GetType() == typeof(DateTime) ? Convert.ToDateTime(z.Value).ToString("dd/MM/yyyy") : z.Value)
               ).ToList();



            return puestoDTO;
        }
    }
}
