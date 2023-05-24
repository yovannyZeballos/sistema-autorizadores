using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioLocalOfiplan : CadenasConexion, IRepositorioLocalOfiplan
    {
        private readonly DBHelper _dbHelper;

        public RepositorioLocalOfiplan(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task Insertar(LocalOfiplan localOfiplan)
        {
            _dbHelper.CadenaConexion = CadenaConexionAutorizadores;

            var dbParams = new OracleParameter[]
            {
                 _dbHelper.MakeParam("cCOD_EMPR",localOfiplan.CodEmpresa,OracleDbType.Varchar2,ParameterDirection.Input),
                 _dbHelper.MakeParam("cCOD_SEDE",localOfiplan.CodSede,OracleDbType.Varchar2,ParameterDirection.Input),
                 _dbHelper.MakeParam("cCOD_CADE",localOfiplan.CodCadenaCt2,OracleDbType.Varchar2,ParameterDirection.Input),
                 _dbHelper.MakeParam("cCOD_LOCA",localOfiplan.CodLocalCt2,OracleDbType.Varchar2,ParameterDirection.Input),
            };

            await _dbHelper.ExecuteNonQuery("PKG_ICT2_AUT_PROCESOS.SP_INS_LOCAL_OFI_CT2", dbParams);
        }

        public async Task<LocalOfiplan> ObtenerLocal(string codEmpresa, string codSede)
        {
            _dbHelper.CadenaConexion = CadenaConexionAutorizadores;

            var dbParams = new OracleParameter[]
            {
                 _dbHelper.MakeParam("cCOD_EMPR",codEmpresa,OracleDbType.Varchar2,ParameterDirection.Input),
                 _dbHelper.MakeParam("cCOD_SEDE",codSede,OracleDbType.Varchar2,ParameterDirection.Input),
                 _dbHelper.MakeParam("p_RECORDSET",1,OracleDbType.RefCursor,ParameterDirection.Output),
            };

            var dr = await _dbHelper.ExecuteReader("PKG_ICT2_AUT_PROCESOS.SP_LISTA_LOCAL_OFI_CT2", dbParams);

            LocalOfiplan local = null;

            if (dr != null && dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    local = new LocalOfiplan(dr["COD_CAD_CT2"].ToString(), dr["COD_LOC_CT2"].ToString(), codEmpresa, codSede);
                }
            }
            dr.Close();

            return local;
        }
    }
}
