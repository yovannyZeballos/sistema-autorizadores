using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Queries.Servidor
{
    public class ListarPlataformasVmQuery : IRequest<GenericResponseDTO<List<SrvPlataformaVm>>>
    {
        public string Texto { get; set; } // opcional: filtrar por nombre
    }

    public class ListarPlataformasVmHandler : IRequestHandler<ListarPlataformasVmQuery, GenericResponseDTO<List<SrvPlataformaVm>>>
    {
        private readonly ISGPContexto _ctx;
        private readonly ILogger _log;

        public ListarPlataformasVmHandler()
        {
            _ctx = new SGPContexto();
            _log = SerilogClass._log;
        }

        public async Task<GenericResponseDTO<List<SrvPlataformaVm>>> Handle(ListarPlataformasVmQuery r, CancellationToken ct)
        {
            var response = new GenericResponseDTO<List<SrvPlataformaVm>>
            {
                Ok = true,
                Data = new List<SrvPlataformaVm>()
            };

            try
            {
                var q = _ctx.RepositorioSrvPlataformaVm.Obtener(_ => true);

                if (!string.IsNullOrWhiteSpace(r.Texto))
                {
                    var t = r.Texto.Trim().ToUpper();
                    q = q.Where(x => (x.NomPlataforma ?? "").ToUpper().Contains(t));
                }

                var lista = await q
                    .OrderBy(x => x.NomPlataforma)
                    .AsNoTracking()
                    .ToListAsync(ct);

                response.Data = lista;
                response.Ok = true;
                response.Mensaje = "Lista de registros obtenido correctamente.";
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error en ListarPlataformasVm");
                response.Ok = false;
                response.Mensaje = ex.Message;
            }
            return response;
        }
    }
}
