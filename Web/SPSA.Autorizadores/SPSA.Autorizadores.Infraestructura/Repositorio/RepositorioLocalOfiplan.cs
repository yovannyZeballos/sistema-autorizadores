using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioLocalOfiplan : CadenasConexion, IRepositorioLocalOfiplan
    {
        private readonly int _commandTimeout;

        public RepositorioLocalOfiplan()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        //public async Task Insertar(LocalOfiplan localOfiplan)
        //{
        //    _dbHelper.CadenaConexion = CadenaConexionAutorizadores;

        //    var dbParams = new OracleParameter[]
        //    {
        //         _dbHelper.MakeParam("cCOD_EMPR",localOfiplan.CodEmpresa,OracleDbType.Varchar2,ParameterDirection.Input),
        //         _dbHelper.MakeParam("cCOD_SEDE",localOfiplan.CodSede,OracleDbType.Varchar2,ParameterDirection.Input),
        //         _dbHelper.MakeParam("cCOD_CADE",localOfiplan.CodCadenaCt2,OracleDbType.Varchar2,ParameterDirection.Input),
        //         _dbHelper.MakeParam("cCOD_LOCA",localOfiplan.CodLocalCt2,OracleDbType.Varchar2,ParameterDirection.Input),
        //    };

        //    await _dbHelper.ExecuteNonQuery("PKG_ICT2_AUT_PROCESOS.SP_INS_LOCAL_OFI_CT2", dbParams);
        //}

        public async Task Insertar(LocalOfiplan localOfiplan)
        {
            const string sql = @"
                                SELECT *
                                FROM ""SGP"".autorizador_fn_ins_local_ofi_ct2(
                                    @cod_empr,
                                    @cod_sede,
                                    @cod_cade,
                                    @cod_loca
                                )";

            using (var cn = new NpgsqlConnection(CadenaConexionSGP))
            using (var cmd = new NpgsqlCommand(sql, cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@cod_empr", (object)localOfiplan.CodEmpresa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cod_sede", (object)localOfiplan.CodSede ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cod_cade", (object)localOfiplan.CodCadenaCt2 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cod_loca", (object)localOfiplan.CodLocalCt2 ?? DBNull.Value);

                await cn.OpenAsync();

                using (var rd = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    if (!await rd.ReadAsync())
                        throw new Exception("La función autorizador_fn_ins_local_ofi_ct2 no devolvió resultado.");

                    var error = rd.GetInt32(0);         // pinnu_error
                    var mensaje = rd.IsDBNull(1) ? "" : rd.GetString(1); // pinvc_msgerr

                    if (error != 0)
                        throw new Exception(mensaje);
                }
            }
        }

        //public async Task<LocalOfiplan> ObtenerLocal(string codEmpresa, string codSede)
        //{
        //    _dbHelper.CadenaConexion = CadenaConexionAutorizadores;

        //    var dbParams = new OracleParameter[]
        //    {
        //         _dbHelper.MakeParam("cCOD_EMPR",codEmpresa,OracleDbType.Varchar2,ParameterDirection.Input),
        //         _dbHelper.MakeParam("cCOD_SEDE",codSede,OracleDbType.Varchar2,ParameterDirection.Input),
        //         _dbHelper.MakeParam("p_RECORDSET",1,OracleDbType.RefCursor,ParameterDirection.Output),
        //    };

        //    var dr = await _dbHelper.ExecuteReader("PKG_ICT2_AUT_PROCESOS.SP_LISTA_LOCAL_OFI_CT2", dbParams);

        //    LocalOfiplan local = null;

        //    if (dr != null && dr.HasRows)
        //    {
        //        while (await dr.ReadAsync())
        //        {
        //            local = new LocalOfiplan(dr["COD_CAD_CT2"].ToString(), dr["COD_LOC_CT2"].ToString(), codEmpresa, codSede);
        //        }
        //    }
        //    dr.Close();

        //    return local;
        //}

        public async Task<LocalOfiplan> ObtenerLocal(string codEmpresa, string codSede)
        {
            const string sql = @"
                                SELECT 
                                    cod_cad_ct2::text AS cod_cad_ct2, 
                                    cod_loc_ct2::text AS cod_loc_ct2
                                FROM ""SGP"".autorizador_fn_lista_local_ofi_ct2(@cod_empr, @cod_sede)
                                LIMIT 1";

            using (var connection = new NpgsqlConnection(CadenaConexionSGP))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = _commandTimeout;

                command.Parameters.AddWithValue("@cod_empr", (object)codEmpresa ?? DBNull.Value);
                command.Parameters.AddWithValue("@cod_sede", (object)codSede ?? DBNull.Value);

                await connection.OpenAsync();

                using (var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    if (await dr.ReadAsync())
                    {
                        var codCadCt2 = dr["cod_cad_ct2"] as string ?? string.Empty;
                        var codLocCt2 = dr["cod_loc_ct2"] as string ?? string.Empty;

                        return new LocalOfiplan(codCadCt2, codLocCt2, codEmpresa, codSede);
                    }
                }
            }

            // No hubo coincidencias
            return null;
        }
    }
}
