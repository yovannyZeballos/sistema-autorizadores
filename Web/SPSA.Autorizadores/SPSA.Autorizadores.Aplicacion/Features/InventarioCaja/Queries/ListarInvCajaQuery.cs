using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Queries;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries
{
    public class ListarInvCajaQuery : IRequest<GenericResponseDTO<List<ListarInvCajaDTO>>>
    {
        public string CodEmpresa { get; set; }
        public string CodCadena { get; set; }
        public string CodRegion { get; set; }
        public string CodZona { get; set; }
        public string CodLocal { get; set; }
        public decimal NumCaja { get; set; }
        public string CodActivo { get; set; }
    }

    public class ListarInvCajaHandler : IRequestHandler<ListarInvCajaQuery, GenericResponseDTO<List<ListarInvCajaDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarInvCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarInvCajaDTO>>> Handle(ListarInvCajaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarInvCajaDTO>> { Ok = true, Data = new List<ListarInvCajaDTO>() };

            try
            {
                var cajas = await _contexto.RepositorioInvCajas.Obtener(x => x.CodEmpresa == request.CodEmpresa
                                                                                    && x.CodCadena == request.CodCadena
                                                                                    && x.CodRegion == request.CodRegion
                                                                                    && x.CodZona == request.CodZona
                                                                                    && x.CodLocal == request.CodLocal
                                                                                    && x.NumCaja == request.NumCaja)
                                                                        .OrderBy(x => x.CodActivo)
                                                                        .ToListAsync();
                response.Data = _mapper.Map<List<ListarInvCajaDTO>>(cajas);
                //response.Ok = true;
                //response.Mensaje = "Se ha generado la lista correctamente";
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
