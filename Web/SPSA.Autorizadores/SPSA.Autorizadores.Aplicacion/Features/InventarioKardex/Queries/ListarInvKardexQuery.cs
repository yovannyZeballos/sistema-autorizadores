using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Linq;
using System.Data.Entity;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries
{
    public class ListarInvKardexQuery : IRequest<GenericResponseDTO<List<ListarInvKardexDTO>>>
    {
        public string Kardex { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class ListarInvKardexHandler : IRequestHandler<ListarInvKardexQuery, GenericResponseDTO<List<ListarInvKardexDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ISGPContexto _contexto;
        private readonly ILogger _logger;
        public ListarInvKardexHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<ListarInvKardexDTO>>> Handle(ListarInvKardexQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<List<ListarInvKardexDTO>> { Ok = true, Data = new List<ListarInvKardexDTO>() };

            try
            {
                List<InvKardex> kardexLista;

                if (request.Kardex == "TODOS") 
                {
                    kardexLista = await _contexto.RepositorioInvKardex.Obtener()
                                                                        .Where(x => x.Fecha >= request.FechaInicio && x.Fecha <= request.FechaFin)
                                                                        .OrderByDescending(x => x.Id)
                                                                        .ToListAsync();
                }
                else
                {
                    kardexLista = await _contexto.RepositorioInvKardex.Obtener()
                                                                        .Where(x => x.Fecha >= request.FechaInicio && x.Fecha <= request.FechaFin)
                                                                        .Where(x => x.Kardex == request.Kardex)
                                                                        .OrderByDescending(x => x.Id)
                                                                        .ToListAsync();
                }
                
                response.Data = _mapper.Map<List<ListarInvKardexDTO>>(kardexLista);
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
