using Npgsql;
using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using NpgsqlTypes;
using System;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using System.Configuration;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMovKardex : CadenasConexion, IRepositorioMovKardex
    {
        private readonly int _commandTimeout;

        public RepositorioMovKardex()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task<DataTable> DescargarMovKardexPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            const string query = @"
                    SELECT *
                    FROM ""SGP"".v_mov_inventario_excel
                    WHERE fecha BETWEEN @FechaInicio AND @FechaFin
                    ORDER BY fecha DESC";

            var dt = new DataTable();

            using (var cn = new NpgsqlConnection(CadenaConexionSGP))
            {
                await cn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, cn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@FechaInicio", NpgsqlDbType.Date) { Value = fechaInicio.Date });
                    cmd.Parameters.Add(new NpgsqlParameter("@FechaFin", NpgsqlDbType.Date) { Value = fechaFin.Date });

                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(rd);
                    }
                }
            }

            return dt;
        }
    }
    //public class RepositorioMovKardex : RepositorioGenerico<SGPContexto, Mov_Kardex>, IRepositorioMovKardex
    //{

    //    public RepositorioMovKardex(SGPContexto context) : base(context) { }

    //    public SGPContexto AppDBMyBDContext
    //    {
    //        get { return _contexto; }
    //    }

    //    //public Task<DataTable> DescargarMovKardexPorFechas(DateTime fechaInicio, DateTime fechaFin)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}

    //    public async Task<DataTable> DescargarMovKardexPorFechas(DateTime fechaInicio, DateTime fechaFin)
    //    {
    //        const string query = @"
    //            SELECT *
    //            FROM ""SGP"".""v_kardex_movimientos""
    //            WHERE fecha BETWEEN @FechaInicio AND @FechaFin
    //            ORDER BY fecha DESC";

    //        var dt = new DataTable();

    //        using (var cn = new NpgsqlConnection(CadenaConexionSGP))
    //        {
    //            await cn.OpenAsync();
    //            using (var cmd = new NpgsqlCommand(query, cn))
    //            {
    //                cmd.Parameters.Add(new NpgsqlParameter("@FechaInicio", NpgsqlDbType.Date) { Value = fechaInicio.Date });
    //                cmd.Parameters.Add(new NpgsqlParameter("@FechaFin", NpgsqlDbType.Date) { Value = fechaFin.Date });

    //                using (var rd = await cmd.ExecuteReaderAsync())
    //                {
    //                    dt.Load(rd);
    //                }
    //            }
    //        }

    //        return dt;
    //    }

    //}
}
