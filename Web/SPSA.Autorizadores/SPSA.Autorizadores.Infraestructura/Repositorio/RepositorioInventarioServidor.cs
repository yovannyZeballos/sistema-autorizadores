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
	public class RepositorioInventarioServidor : CadenasConexion, IRepositorioInventarioServidor
	{
		private readonly DBHelper _dbHelper;

		public RepositorioInventarioServidor(DBHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}
		public async Task Insertar(InventarioServidor inventarioServidor)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@COD_EMPRESA",inventarioServidor.CodEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_FORMATO",inventarioServidor.CodFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_LOCAL",inventarioServidor.CodLocal,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@NUM_SERVER",inventarioServidor.NumServer,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@TIP_SERVER",inventarioServidor.TipoServer,SqlDbType.VarChar,ParameterDirection.Input,2),
				_dbHelper.MakeParam("@COD_MARCA",inventarioServidor.CodMarca,SqlDbType.VarChar,ParameterDirection.Input,2),
				_dbHelper.MakeParam("@COD_MODELO",inventarioServidor.CodModelo,SqlDbType.VarChar,ParameterDirection.Input,2),
				_dbHelper.MakeParam("@HOSTNAME",inventarioServidor.Hostname,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@SERIE",inventarioServidor.Serie,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@IP",inventarioServidor.Ip,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@RAM",inventarioServidor.Ram,SqlDbType.Decimal,ParameterDirection.Input),
				_dbHelper.MakeParam("@HDD",inventarioServidor.Hdd,SqlDbType.Decimal,ParameterDirection.Input),
				_dbHelper.MakeParam("@COD_SO",inventarioServidor.CodSo,SqlDbType.VarChar,ParameterDirection.Input,2),
				_dbHelper.MakeParam("@REPLICA",inventarioServidor.Replica,SqlDbType.VarChar,ParameterDirection.Input,1),
				_dbHelper.MakeParam("@IP_REMOTA",inventarioServidor.IpRemota,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@ANTIGUEDAD",inventarioServidor.Antiguedad,SqlDbType.Decimal,ParameterDirection.Input),
				_dbHelper.MakeParam("@OBSERVACIONES",inventarioServidor.Observaciones,SqlDbType.VarChar,ParameterDirection.Input,250),
				_dbHelper.MakeParam("@ANTIVIRUS",inventarioServidor.Antivirus,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@USUARIO",inventarioServidor.Usuario,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@ERROR",0,SqlDbType.Int,ParameterDirection.Output),
				_dbHelper.MakeParam("@MSGERR","",SqlDbType.VarChar,ParameterDirection.Output,250),
			};

			var outputs = await _dbHelper.ExecuteNonQueryWithOutput("SP_LOC_SERV_INV_INSERTAR", dbParams);
			var error = Convert.ToInt32(outputs["@ERROR"].ToString());
			var mensajeError = outputs["@MSGERR"].ToString();

			if (error > 0)
				throw new Exception(mensajeError);
		}

		public async Task<DataTable> DescargarMaestro(string codEmpresa, string codFormato, string codLocal)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",codFormato ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_LOCAL",codLocal ?? "",SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var dr = await _dbHelper.ExecuteReader("SP_LOC_SERV_INV_DESCARGAR_MAESTRO", dbParams);
			var dt = new DataTable();
			dt.Load(dr);
			dr.Close();
			return dt;
		}

		public async Task<DataTable> Listar(string codEmpresa, string codFormato, string codLocal)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",codFormato ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_LOCAL",codLocal ?? "",SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var dr = await _dbHelper.ExecuteReader("SP_LOC_SERV_INV_LISTAR", dbParams);
			var dt = new DataTable();
			dt.Load(dr);
			dr.Close();
			return dt;
		}

		public async Task<InventarioServidor> Obtener(string codEmpresa, string codFormato, string codLocal, string numServer)
		{
			InventarioServidor inventario = null;

			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@COD_EMPRESA",codEmpresa ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_FORMATO",codFormato ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_LOCAL",codLocal ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@NUM_SERVER",numServer ?? "",SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			using (var dr = await _dbHelper.ExecuteReader("SP_LOC_SERV_INV_OBTENER", dbParams))
			{
				if (dr != null && dr.HasRows)
				{
					while (await dr.ReadAsync())
					{
						inventario = new InventarioServidor
						(
							dr["COD_EMPRESA"].ToString(),
							dr["COD_FORMATO"].ToString(),
							dr["COD_LOCAL"].ToString(),
							dr["NUM_SERVER"].ToString(),
							dr["TIP_SERVER"].ToString(),
							dr["COD_MARCA"].ToString(),
							dr["COD_MODELO"].ToString(),
							dr["HOSTNAME"].ToString(),
							dr["SERIE"].ToString(),
							dr["IP"].ToString(),
							Convert.ToDecimal(dr["RAM"]),
							Convert.ToDecimal(dr["HDD"]),
							dr["COD_SO"].ToString(),
							dr["REPLICA"].ToString(),
							dr["IP_REMOTA"].ToString(),
							Convert.ToDecimal(dr["ANTIGUEDAD"]),
							dr["OBSERVACIONES"].ToString(),
							dr["ANTIVIRUS"].ToString(),
							dr["USU_CREACION"].ToString() ,
							dr["USU_MODIFICA"].ToString(),
							dr["FECHA_CREACION"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FECHA_CREACION"]),
						    dr["FECHA_MODIFICA"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FECHA_MODIFICA"])
						);
					}
				}
			}
			return inventario;
		}
	}
}
