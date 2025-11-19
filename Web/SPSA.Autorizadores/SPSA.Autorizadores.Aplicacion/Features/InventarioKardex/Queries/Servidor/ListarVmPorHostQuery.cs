using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Servidor;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Servidor
{
    public class ListarVmPorHostQuery : IRequest<GenericResponseDTO<PagedResult<ServidorVirtualDto>>>
    {
        public long HostSerieId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Texto { get; set; }       // búsqueda libre: hostname, ip, notas, so, plataforma
        public string IndActivo { get; set; }   // 'S' / 'N' / null (todos)
    }

    public class ListarVmPorHostHandler : IRequestHandler<ListarVmPorHostQuery, GenericResponseDTO<PagedResult<ServidorVirtualDto>>>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public ListarVmPorHostHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ServidorVirtualDto>>> Handle(ListarVmPorHostQuery r, CancellationToken ct)
        {
            var resp = new GenericResponseDTO<PagedResult<ServidorVirtualDto>>
            {
                Ok = true,
                Data = new PagedResult<ServidorVirtualDto>()
            };

            try
            {
                if (r.HostSerieId <= 0)
                {
                    resp.Ok = false;
                    resp.Mensaje = "Host inválido.";
                    return resp;
                }

                // 1) Queries base FUERA del árbol de expresión
                var vmQ = _ctx.RepositorioSrvVirtual.Obtener(v => v.HostSerieId == r.HostSerieId);
                var soQ = _ctx.RepositorioSrvSistemaOperativo.Obtener(_ => true);
                var plQ = _ctx.RepositorioSrvPlataformaVm.Obtener(_ => true);

                // 2) LEFT JOINs usando esas variables (tipos iguales: nullable vs nullable)
                var q =
                    from vm in vmQ
                    join so in soQ on vm.SoId equals (long?)so.Id into sox
                    from so in sox.DefaultIfEmpty()
                    join pl in plQ on vm.PlataformaId equals (short?)pl.Id into plx
                    from pl in plx.DefaultIfEmpty()
                    select new { vm, so, pl };

                // 3) Filtro activo
                if (!string.IsNullOrWhiteSpace(r.IndActivo))
                {
                    var ia = r.IndActivo.Trim().ToUpper();
                    if (ia == "S" || ia == "N")
                        q = q.Where(x => x.vm.IndActivo == ia);
                }

                // 4) Búsqueda libre
                if (!string.IsNullOrWhiteSpace(r.Texto))
                {
                    var t = r.Texto.Trim().ToUpper();
                    q = q.Where(x =>
                        ((x.vm.Hostname ?? "").ToUpper().Contains(t)) ||
                        ((x.vm.Url ?? "").ToUpper().Contains(t)) ||
                        ((x.so.NomSo ?? "").ToUpper().Contains(t)) ||
                        ((x.pl.NomPlataforma ?? "").ToUpper().Contains(t))
                    );
                }

                // 5) Conteo + paginación + proyección
                var total = await q.CountAsync(ct);

                var items = await q
                    .OrderBy(x => x.vm.Hostname)
                    .ThenBy(x => x.vm.Id)
                    .Skip((r.PageNumber - 1) * r.PageSize)
                    .Take(r.PageSize)
                    .Select(x => new ServidorVirtualDto
                    {
                        Id = x.vm.Id,
                        HostSerieId = x.vm.HostSerieId,
                        Hostname = x.vm.Hostname,
                        Ip = x.vm.Ip,
                        RamGb = x.vm.RamGb,
                        VCores = x.vm.VCores,
                        HddTotal = x.vm.HddTotal,
                        SoId = x.vm.SoId,
                        NomSo = x.so != null ? x.so.NomSo : null,
                        PlataformaId = x.vm.PlataformaId,
                        NomPlataforma = x.pl != null ? x.pl.NomPlataforma : null,
                        IndActivo = x.vm.IndActivo,
                        Url = x.vm.Url,
                        UsuCreacion = x.vm.UsuCreacion,
                        FecCreacion = x.vm.FecCreacion,
                        UsuModifica = x.vm.UsuModifica,
                        FecModifica = x.vm.FecModifica
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);

                resp.Data.TotalRecords = total;
                resp.Data.Items = items;
                return resp;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error en ListarVmPorHost");
                return new GenericResponseDTO<PagedResult<ServidorVirtualDto>>
                {
                    Ok = false,
                    Mensaje = ex.Message
                };
            }
        }
    }
}
