using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Autorizadores.Queries
{

    public class ListarColaboradoresQuery : IRequest<DataTable>
    {
        public string CodigoLocal { get; set; } = string.Empty;
        public string CodigoEmpresa { get; set; } = string.Empty;
    }

    public class ListarColaboradoresHandler : IRequestHandler<ListarColaboradoresQuery, DataTable>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAutorizadores _repositorioAutorizadores;

        public ListarColaboradoresHandler(IMapper mapper, IRepositorioEmpresa repositorioEmpresa , IRepositorioAutorizadores repositorioAutorizadores)
        {
            _mapper = mapper;
            _repositorioAutorizadores = repositorioAutorizadores;
        }

        public async Task<DataTable> Handle(ListarColaboradoresQuery request, CancellationToken cancellationToken)
        {
            var colaboradoresDatatable = await _repositorioAutorizadores.ListarColaboradores(request.CodigoLocal, request.CodigoEmpresa);
            return colaboradoresDatatable;
        }
    }
}
