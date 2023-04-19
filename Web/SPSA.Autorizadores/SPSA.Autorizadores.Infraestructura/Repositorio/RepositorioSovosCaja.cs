using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioSovosCaja : CadenasConexion, IRepositorioSovosCaja
    {
        private readonly DBHelper _dbHelper;

        public RepositorioSovosCaja(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task Crear(SovosCaja sovosCaja)
        {
            _dbHelper.CadenaConexion = CadenaConexionCarteleria;

            SqlParameter[] dbParams = new SqlParameter[]
            {
                 _dbHelper.MakeParam("@COD_EMPRESA",sovosCaja.CodEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
                 _dbHelper.MakeParam("@COD_FORMATO",sovosCaja.CodFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
                 _dbHelper.MakeParam("@COD_LOCAL",sovosCaja.CodLocal,SqlDbType.VarChar,ParameterDirection.Input,10),
                 _dbHelper.MakeParam("@NUM_POS",sovosCaja.NumeroCaja,SqlDbType.Decimal,ParameterDirection.Input),
                 _dbHelper.MakeParam("@IP_ADDRES",sovosCaja.Ip,SqlDbType.VarChar,ParameterDirection.Input,30),
                 _dbHelper.MakeParam("@TIP_OS",sovosCaja.So,SqlDbType.VarChar,ParameterDirection.Input,1),
                 _dbHelper.MakeParam("@ERROR",0,SqlDbType.Int,ParameterDirection.Output),
                 _dbHelper.MakeParam("@MSGERR","",SqlDbType.VarChar,ParameterDirection.Output,250)
            };

            var outputs = await _dbHelper.ExecuteNonQueryWithOutput("SP_LOC_CAJA_SAVE", dbParams);

            var error = Convert.ToInt32(outputs["@ERROR"].ToString());
            var mensajeError = outputs["@MSGERR"].ToString();

            if (error > 0)
                throw new Exception(mensajeError);
        }

        public async Task Eliminar(string codEmpresa, string codFormato, string codLocal, string cajas)
        {
            _dbHelper.CadenaConexion = CadenaConexionCarteleria;

            SqlParameter[] dbParams = new SqlParameter[]
            {
                 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
                 _dbHelper.MakeParam("@COD_FORMATO",codFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
                 _dbHelper.MakeParam("@COD_LOCAL",codLocal,SqlDbType.VarChar,ParameterDirection.Input,10),
                 _dbHelper.MakeParam("@NUM_POS",cajas,SqlDbType.VarChar,ParameterDirection.Input),
                 _dbHelper.MakeParam("@ERROR",0,SqlDbType.Int,ParameterDirection.Output),
                 _dbHelper.MakeParam("@MSGERR","",SqlDbType.VarChar,ParameterDirection.Output,250)
            };

            var outputs = await _dbHelper.ExecuteNonQueryWithOutput("SP_LOC_CAJA_DELE", dbParams);

            var error = Convert.ToInt32(outputs["@ERROR"].ToString());
            var mensajeError = outputs["@MSGERR"].ToString();

            if (error > 0)
                throw new Exception(mensajeError);
        }

        public async Task<DataTable> ListarPorLocal(string codEmpresa, string codFormato, string codLocal)
        {
            _dbHelper.CadenaConexion = CadenaConexionCarteleria;

            SqlParameter[] dbParams = new SqlParameter[]
            {
                 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
                 _dbHelper.MakeParam("@COD_FORMATO",codFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
                 _dbHelper.MakeParam("@COD_LOCAL",codLocal,SqlDbType.VarChar,ParameterDirection.Input,10),
            };

            var dr = await _dbHelper.ExecuteReader("SP_LOC_CAJAS_XLOCAL", dbParams);
            var dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return dt;
        }
    }
}
