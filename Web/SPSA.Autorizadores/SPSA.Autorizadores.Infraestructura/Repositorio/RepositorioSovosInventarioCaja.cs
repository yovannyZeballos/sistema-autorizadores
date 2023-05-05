using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioSovosInventarioCaja : CadenasConexion, IRepositorioSovosInventarioCaja
    {
        private readonly DBHelper _dbHelper;

        public RepositorioSovosInventarioCaja(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task Insertar(DataTable dt)
        {
            _dbHelper.CadenaConexion = CadenaConexionCarteleria;
            SqlParameter[] dbParams = new SqlParameter[]
            {
                _dbHelper.MakeParam("@DATA",dt,SqlDbType.Structured,ParameterDirection.Input)
            };

            await _dbHelper.ExecuteNonQuery("SP_LOC_CARGA_INV_POS", dbParams);
        }
    }
}
