using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.DTOs;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Queries
{
    public class ObtenerListasInvCajaQuery : IRequest<GenericResponseDTO<ObtenerListasInvCajaDTO>>
    {
        public string CodEmpresa { get; set; }
    }

    public class ObtenerListasInvCajaHandler : IRequestHandler<ObtenerListasInvCajaQuery, GenericResponseDTO<ObtenerListasInvCajaDTO>>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ObtenerListasInvCajaHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<ObtenerListasInvCajaDTO>> Handle(ObtenerListasInvCajaQuery request, CancellationToken cancellationToken)
        {
            var response = new GenericResponseDTO<ObtenerListasInvCajaDTO> { Ok = true, Data = new ObtenerListasInvCajaDTO() };
            try
            {
                var listaModelo = await _contexto.RepositorioInvCajas.Obtener(x => x.CodEmpresa == request.CodEmpresa)
                   .Select(x => x.CodModelo)
                   .Distinct()
                   .OrderBy(m => m)
                   .ToListAsync();

                var listaProcesador = await _contexto.RepositorioInvCajas.Obtener(x => !string.IsNullOrEmpty(x.TipProcesador) && x.CodEmpresa == request.CodEmpresa )
                    .Select(x => x.TipProcesador)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToListAsync();

                var listaMemoria = await _contexto.RepositorioInvCajas.Obtener(x => !string.IsNullOrEmpty(x.Memoria) && x.CodEmpresa == request.CodEmpresa)
                    .Select(x => x.Memoria)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToListAsync();

                var listaSo = await _contexto.RepositorioInvCajas.Obtener(x => !string.IsNullOrEmpty(x.DesSo) && x.CodEmpresa == request.CodEmpresa)
                    .Select(x => x.DesSo)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToListAsync();

                listaSo.Add("SUSE Linux Enterprise Server 15 SP6");

                var listaVerSo = await _contexto.RepositorioInvCajas.Obtener(x => !string.IsNullOrEmpty(x.VerSo))
                    .Select(x => x.VerSo)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToListAsync();

                var listaCapDisco = await _contexto.RepositorioInvCajas.Obtener(x => !string.IsNullOrEmpty(x.CapDisco))
                    .Select(x => x.CapDisco)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                var listaPuertoBal = await _contexto.RepositorioInvCajas.Obtener(x => !string.IsNullOrEmpty(x.DesPuertoBalanza))
                    .Select(x => x.DesPuertoBalanza)
                    .Distinct()
                    .OrderBy(b => b)
                    .ToListAsync();

                response.Data.TiposModelo = listaModelo;
                response.Data.TiposProcesador = listaProcesador;
                response.Data.TiposMemoria = listaMemoria;
                response.Data.TiposSo = listaSo;
                response.Data.TiposVerSo = listaVerSo;
                response.Data.TiposCapDisco = listaCapDisco;
                response.Data.TiposPuertoBalanza = listaPuertoBal;

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
