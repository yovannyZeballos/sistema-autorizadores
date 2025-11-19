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
    public class ListarPaginadoServidoresInventarioQuery : IRequest<GenericResponseDTO<PagedResult<ServidorInventarioDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Filtros
        public string CodEmpresa { get; set; }
        public string CodLocal { get; set; }
        public string IndEstadoSerie { get; set; } // DISPONIBLE/EN_TRANSITO/EN_USO/DE_BAJA
        public short? TipoId { get; set; }         // srv_tipo_servidor.id
        public string Texto { get; set; }          // búsqueda libre (hostname, serie, ip, producto, marca, modelo, tipo)
    }

    public class ListarPaginadoServidoresInventarioHandler : IRequestHandler<ListarPaginadoServidoresInventarioQuery, GenericResponseDTO<PagedResult<ServidorInventarioDto>>>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public ListarPaginadoServidoresInventarioHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<PagedResult<ServidorInventarioDto>>> Handle(ListarPaginadoServidoresInventarioQuery request, CancellationToken ct)
        {
            var resp = new GenericResponseDTO<PagedResult<ServidorInventarioDto>>
            {
                Ok = true,
                Data = new PagedResult<ServidorInventarioDto>()
            };

            try
            {
                // Aliases de repos (ajusta a tus nombres reales si difieren)
                // var repSerie  = _ctx.RepositorioMaeSerieProducto;
                // var repProd   = _ctx.RepositorioMaeProducto;
                // var repAG     = _ctx.RepositorioMaeAreaGestion;
                // var repMarca  = _ctx.RepositorioMaeMarca;
                // var repSrvDet = _ctx.RepositorioSrvSerieCaracteristica; // o RepositorioSrvServidor si así se llama tu entidad
                // var repTipo   = _ctx.RepositorioSrvTipoServidor;
                // var repLocal  = _ctx.RepositorioMaeLocal;
                // var repEmp    = _ctx.RepositorioMaeEmpresa;

                var q =
                    from sp in _ctx.RepositorioMaeSerieProducto.Obtener(_ => true)
                    join p in _ctx.RepositorioMaeProducto.Obtener(_ => true)
                        on sp.CodProducto equals p.CodProducto
                    join ag in _ctx.RepositorioMaeAreaGestion.Obtener(_ => true)
                        on p.AreaGestionId equals ag.Id into agx
                    from ag in agx.DefaultIfEmpty()
                    join m in _ctx.RepositorioMaeMarca.Obtener(_ => true)
                        on p.MarcaId equals m.Id into mx
                    from m in mx.DefaultIfEmpty()
                    join s in _ctx.RepositorioSrvSerieCaracteristica.Obtener(_ => true)
                        on sp.Id equals s.SerieProductoId into sx
                    from s in sx.DefaultIfEmpty()
                    join ts in _ctx.RepositorioSrvTipoServidor.Obtener(_ => true)
                        on s.TipoId equals ts.Id into tsx
                    from ts in tsx.DefaultIfEmpty()
                        // 👇 JOIN al catálogo de Sistema Operativo
                    join so in _ctx.RepositorioSrvSistemaOperativo.Obtener(_ => true)
                        on s.SoId equals so.Id into sox
                    from so in sox.DefaultIfEmpty()
                    join l in _ctx.RepositorioMaeLocal.Obtener(_ => true)
                        on new { Emp = sp.CodEmpresa, Loc = sp.CodLocal }
                        equals new { Emp = l.CodEmpresa, Loc = l.CodLocal } into lx
                    from l in lx.DefaultIfEmpty()
                    join e in _ctx.RepositorioMaeEmpresa.Obtener(_ => true)
                        on sp.CodEmpresa equals e.CodEmpresa into ex
                    from e in ex.DefaultIfEmpty()
                    where p.AreaGestionId == 4 // ← servidores
                    select new
                    {
                        sp,
                        p,
                        ag,
                        m,
                        s,
                        ts,
                        so,
                        l,
                        e
                    };

                // ===== Filtros =====
                if (!string.IsNullOrWhiteSpace(request.CodEmpresa))
                    q = q.Where(x => x.sp.CodEmpresa == request.CodEmpresa);

                if (!string.IsNullOrWhiteSpace(request.CodLocal))
                    q = q.Where(x => x.sp.CodLocal == request.CodLocal);

                if (!string.IsNullOrWhiteSpace(request.IndEstadoSerie))
                    q = q.Where(x => x.sp.IndEstado == request.IndEstadoSerie);

                if (request.TipoId.HasValue && request.TipoId.Value > 0)
                    q = q.Where(x => x.s != null && x.s.TipoId == request.TipoId.Value);

                if (!string.IsNullOrWhiteSpace(request.Texto))
                {
                    var t = request.Texto.Trim().ToUpper();
                    q = q.Where(x =>
                        ((x.s.Hostname ?? "").ToUpper().Contains(t)) ||
                        ((x.sp.NumSerie ?? "").ToUpper().Contains(t)) ||
                        ((x.p.DesProducto ?? "").ToUpper().Contains(t)) ||
                        ((x.m.NomMarca ?? "").ToUpper().Contains(t)) ||
                        ((x.p.NomModelo ?? "").ToUpper().Contains(t)) ||
                        ((x.ts.NomTipo ?? "").ToUpper().Contains(t)) ||
                        ((x.so.NomSo ?? "").ToUpper().Contains(t))
                    );
                }

                // Conteo total
                var total = await q.CountAsync(ct);

                // Paginación + proyección final
                var items = await q
                    .OrderBy(x => x.s.Hostname ?? x.sp.NumSerie)  // primero hostname…
                    .ThenBy(x => x.sp.NumSerie)  // …luego serie
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new ServidorInventarioDto
                    {
                        SerieProductoId = x.sp.Id,
                        CodProducto = x.sp.CodProducto,
                        AreaGestionId = x.p.AreaGestionId,
                        NomAreaGestion = x.ag != null ? x.ag.NomAreaGestion : null,
                        DesProducto = x.p.DesProducto,
                        NomMarca = x.m != null ? x.m.NomMarca : null,
                        Modelo = x.p.NomModelo,

                        NumSerie = x.sp.NumSerie,
                        IndEstadoSerie = x.sp.IndEstado,
                        CodEmpresa = x.sp.CodEmpresa,
                        CodLocal = x.sp.CodLocal,
                        StkActual = x.sp.StkActual,

                        TipoId = x.s != null ? (short?)x.s.TipoId : null,
                        TipoServidor = x.ts != null ? x.ts.NomTipo : null,

                        SoId = x.s != null ? x.s.SoId : null,
                        NomSo = x.so != null ? x.so.NomSo : null,

                        Hostname = x.s != null ? x.s.Hostname : null,
                        IpSo = x.s != null ? x.s.IpSo : null,
                        RamGb = x.s != null ? x.s.RamGb : null,
                        CpuSockets = x.s != null ? x.s.CpuSockets : null,
                        CpuCores = x.s != null ? x.s.CpuCores : null,
                        HddTotal = x.s != null ? x.s.HddTotal : null,

                        MacAddress = x.s != null ? x.s.MacAddress : null,
                        FecIngreso = x.s != null ? x.s.FecIngreso : (DateTime?)null,
                        Antiguedad = x.s != null ? (int?)x.s.Antiguedad : (int?)null,
                        ConexionRemota = x.s != null ? x.s.ConexionRemota : null,
                        IpRemota = x.s != null ? x.s.IpRemota : null,

                        NomLocal = x.l != null ? x.l.NomLocal : null,
                        NomEmpresa = x.e != null ? x.e.NomEmpresa : null
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);

                resp.Data.TotalRecords = total;
                resp.Data.Items = items;
                return resp;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error en ListarPaginadoServidoresInventario");
                return new GenericResponseDTO<PagedResult<ServidorInventarioDto>>
                {
                    Ok = false,
                    Mensaje = ex.Message
                };
            }
        }

    }
}
