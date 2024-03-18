using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Queries;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Infraestructura.Contexto;
using Serilog;
using System.Data.Entity;
using System.Linq;

namespace SPSA.Autorizadores.Aplicacion.Features.Caja.Queries
{
    public class ListarMaeCajaQuery : IRequest<GenericResponseDTO<List<ListarMaeCajaDTO>>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
    }

    public class ListarMaeCajaHandler : IRequestHandler<ListarMaeCajaQuery, GenericResponseDTO<List<ListarMaeCajaDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IBCTContexto _contexto;
        private readonly ILogger _logger;
        public ListarMaeCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new BCTContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarMaeCajaDTO>>> Handle(ListarMaeCajaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarMaeCajaDTO>> { Ok = true, Data = new List<ListarMaeCajaDTO>() };
            try
            {
                var cajas = await _contexto.RepositorioMaeCaja.Obtener(x => x.CodEmpresa == request.CodEmpresa && x.CodCadena == request.CodCadena && x.CodRegion == request.CodRegion && x.CodZona == request.CodZona && x.CodLocal == request.CodLocal)
                    .OrderBy(x => x.NumCaja)
                    .ToListAsync();
                response.Data = _mapper.Map<List<ListarMaeCajaDTO>>(cajas);
                response.Ok = true;
                response.Mensaje = "Se ha generado la lista correctamente";
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensaje = ex.Message;
                _logger.Error(ex, response.Mensaje);
            }
            return response;
        }
    }

}
