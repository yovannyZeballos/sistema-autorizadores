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
	public class RepositorioInventarioServidorVirtual : CadenasConexion, IRepositorioInventarioServidorVirtual
	{
		private readonly DBHelper _dbHelper;

		public RepositorioInventarioServidorVirtual(DBHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}

		public async Task EliminarPorIds(string ids)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@ID",ids,SqlDbType.VarChar,ParameterDirection.Input),
				_dbHelper.MakeParam("@ERROR",0,SqlDbType.Int,ParameterDirection.Output),
				_dbHelper.MakeParam("@MSGERR","",SqlDbType.VarChar,ParameterDirection.Output,250),
			};

			var outputs = await _dbHelper.ExecuteNonQueryWithOutput("SP_LOC_SERV_INV_VIRTUALES_DELE", dbParams);
			var error = Convert.ToInt32(outputs["@ERROR"].ToString());
			var mensajeError = outputs["@MSGERR"].ToString();

			if (error > 0)
				throw new Exception(mensajeError);
		}

		public async Task Insertar(InventarioServidorVirtual inventarioServidorVirtual)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@ID",inventarioServidorVirtual.Id,SqlDbType.Int,ParameterDirection.Input),
				_dbHelper.MakeParam("@COD_EMPRESA",inventarioServidorVirtual.CodEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_FORMATO",inventarioServidorVirtual.CodFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_LOCAL",inventarioServidorVirtual.CodLocal,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@NUM_SERVER",inventarioServidorVirtual.NumServer,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@VIRTUAL_TIPO",inventarioServidorVirtual.Tipo,SqlDbType.VarChar,ParameterDirection.Input,2),
				_dbHelper.MakeParam("@VIRTUAL_RAM",inventarioServidorVirtual.Ram,SqlDbType.Decimal,ParameterDirection.Input),
				_dbHelper.MakeParam("@VIRTUAL_CPU",inventarioServidorVirtual.Cpu,SqlDbType.Decimal,ParameterDirection.Input),
				_dbHelper.MakeParam("@VIRTUAL_HDD",inventarioServidorVirtual.Hdd,SqlDbType.Decimal,ParameterDirection.Input),
				_dbHelper.MakeParam("@VIRTUAL_SO",inventarioServidorVirtual.So,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@USUARIO",inventarioServidorVirtual.Usuario,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@ERROR",0,SqlDbType.Int,ParameterDirection.Output),
				_dbHelper.MakeParam("@MSGERR","",SqlDbType.VarChar,ParameterDirection.Output,250),
			};

			var outputs = await _dbHelper.ExecuteNonQueryWithOutput("SP_LOC_SERV_INV_INSERTAR_VIRTUAL", dbParams);
			var error = Convert.ToInt32(outputs["@ERROR"].ToString());
			var mensajeError = outputs["@MSGERR"].ToString();

			if (error > 0)
				throw new Exception(mensajeError);
		}

		public async Task<DataTable> Listar(string codEmpresa, string codFormato, string codLocal, string numeroServidor)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",codFormato ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_LOCAL",codLocal ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@NUM_SERVER",numeroServidor ?? "",SqlDbType.VarChar,ParameterDirection.Input,20)
			};

			var dr = await _dbHelper.ExecuteReader("SP_LOC_SERV_INV_LISTAR_VIRTUALES", dbParams);
			var dt = new DataTable();
			dt.Load(dr);
			dr.Close();
			return dt;
		}
	}
}
