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

		public async Task<DataTable> DescargarMaestro(string codEmpresa, string codFormato, string codLocal)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",codFormato ?? "",SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_LOCAL",codLocal ?? "",SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var dr = await _dbHelper.ExecuteReader("SP_LOC_CAJAS_INV_DESCARGAR_MAESTRO", dbParams);
			var dt = new DataTable();
			dt.Load(dr);
			dr.Close();
			return dt;
		}

		public async Task Insertar(SovosCajaInventario sovosCajaInventario)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@COD_EMPRESA",sovosCajaInventario.CodEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_FORMATO",sovosCajaInventario.CodFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_LOCAL",sovosCajaInventario.CodLocal,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@NUM_POS",sovosCajaInventario.NumPos,SqlDbType.Decimal,ParameterDirection.Input),
				_dbHelper.MakeParam("@RANKING",sovosCajaInventario.Ranking,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@ESTADO",sovosCajaInventario.Estado,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@SEDE",sovosCajaInventario.Sede,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@UBICACION",sovosCajaInventario.Ubicacion,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@CAJA",sovosCajaInventario.Caja,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@MODELO_CPU",sovosCajaInventario.ModeloCpu,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@SERIE",sovosCajaInventario.Serie,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@MODELO_PRINT",sovosCajaInventario.ModeloPrint,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@SERIE_PRINT",sovosCajaInventario.SeriePrint,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@MODELO_DYNAKEY",sovosCajaInventario.ModeloDinakey,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@SERIE_DYNAKEY",sovosCajaInventario.SerieDinakey,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@MODELO_SCANNER",sovosCajaInventario.ModeloScanner,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@SERIE_SCANNER",sovosCajaInventario.SerieScanner,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@MODELO_GAVETA",sovosCajaInventario.ModeloGaveta,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@SERIE_GAVETA",sovosCajaInventario.SerieGaveta,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@MODELO_MONITOR",sovosCajaInventario.ModeloMonitor,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@SERIE_MONITOR",sovosCajaInventario.SerieMonitor,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@FECHA_APERTURA",(object)sovosCajaInventario.FechaApertura ?? DBNull.Value,SqlDbType.DateTime,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@CARACT_1",sovosCajaInventario.Caract1,SqlDbType.VarChar,ParameterDirection.Input,2),
				_dbHelper.MakeParam("@CARACT_2",sovosCajaInventario.Caract2,SqlDbType.VarChar,ParameterDirection.Input,2),
				_dbHelper.MakeParam("@CARACT_3",sovosCajaInventario.Caract3,SqlDbType.VarChar,ParameterDirection.Input,2),
				_dbHelper.MakeParam("@FECHA_LISING",(object)sovosCajaInventario.FechaLising ?? DBNull.Value,SqlDbType.DateTime,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@SO",sovosCajaInventario.So,SqlDbType.VarChar,ParameterDirection.Input,1),
				_dbHelper.MakeParam("@VERSION_SO",sovosCajaInventario.VesionSo,SqlDbType.VarChar,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@FECHA_ASIGNACION",(object)sovosCajaInventario.FechaAsignacion ?? DBNull.Value,SqlDbType.DateTime,ParameterDirection.Input,100),
				_dbHelper.MakeParam("@USUARIO",sovosCajaInventario.Usuario,SqlDbType.VarChar,ParameterDirection.Input,20),
				_dbHelper.MakeParam("@ERROR",0,SqlDbType.Int,ParameterDirection.Output),
				_dbHelper.MakeParam("@MSGERR","",SqlDbType.VarChar,ParameterDirection.Output,250),
			};

			var outputs = await _dbHelper.ExecuteNonQueryWithOutput("SP_LOC_CAJA_INV_INSERTAR", dbParams);
			var error = Convert.ToInt32(outputs["@ERROR"].ToString());
			var mensajeError = outputs["@MSGERR"].ToString();

			if (error > 0)
				throw new Exception(mensajeError);
		}

		public async Task InsertarMasivo(DataTable dt)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@DATA",dt,SqlDbType.Structured,ParameterDirection.Input)
			};

			await _dbHelper.ExecuteNonQuery("SP_LOC_CARGA_INV_POS", dbParams);
		}

		public async Task<DataTable> Listar()
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = null;
			var dr = await _dbHelper.ExecuteReader("SP_LOC_CAJA_INV_LISTAR", dbParams);
			var dt = new DataTable();
			dt.Load(dr);
			dr.Close();
			return dt;
		}

		public async Task<List<CaracteristicaCaja>> ListarCaracteristicas(string codEmpresa, int tipo)
		{
			var caracteristicas = new List<CaracteristicaCaja>();
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@TIPO",tipo,SqlDbType.Int,ParameterDirection.Input)
			};

			using (var dr = await _dbHelper.ExecuteReader("SP_LOC_LISTAR_CARACT", dbParams))
			{
				if (dr != null && dr.HasRows)
				{
					while (await dr.ReadAsync())
					{
						caracteristicas.Add(new CaracteristicaCaja(Convert.ToInt32(dr["Id"]), dr["Descripcion"].ToString()));
					}
				}
			}

			return caracteristicas;
		}

		public async Task<SovosCajaInventario> Obtener(string codEmpresa, string codFormato, string codLocal, decimal numPos)
		{
			SovosCajaInventario cajaInvetario = null;
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_FORMATO",codFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@COD_LOCAL",codLocal,SqlDbType.VarChar,ParameterDirection.Input,10),
				_dbHelper.MakeParam("@NUM_POS",numPos,SqlDbType.Int,ParameterDirection.Input)
			};

			using (var dr = await _dbHelper.ExecuteReader("SP_LOC_CAJA_INV_OBTENER", dbParams))
			{
				if (dr != null && dr.HasRows)
				{
					while (await dr.ReadAsync())
					{
						cajaInvetario = new SovosCajaInventario(codEmpresa, codFormato, codLocal, numPos, dr["RANKING"].ToString(), dr["ESTADO"].ToString(),
							dr["SEDE"].ToString(), dr["UBICACION"].ToString(), dr["CAJA"].ToString(), dr["MODELO_CPU"].ToString(), dr["SERIE"].ToString(),
							dr["MODELO_PRINT"].ToString(), dr["SERIE_PRINT"].ToString(), dr["MODELO_DYNAKEY"].ToString(), dr["SERIE_DYNAKEY"].ToString(),
							dr["MODELO_SCANNER"].ToString(), dr["SERIE_SCANNER"].ToString(), dr["MODELO_GAVETA"].ToString(), dr["SERIE_GAVETA"].ToString(),
							dr["MODELO_MONITOR"].ToString(), dr["SERIE_MONITOR"].ToString(), dr["FECHA_APERTURA"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FECHA_APERTURA"]),
							dr["CARACT_1"].ToString(), dr["CARACT_2"].ToString(), dr["CARACT_3"].ToString(), dr["FECHA_LISING"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FECHA_LISING"]),
							dr["SO"].ToString(), dr["VERSION_SO"].ToString(), dr["FECHA_ASIGNACION"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FECHA_ASIGNACION"]),
							dr["USU_CREACION"].ToString(), dr["USU_MODIFICA"].ToString(), dr["FECHA_CREACION"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FECHA_CREACION"]),
							 dr["FECHA_MODIFICA"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FECHA_MODIFICA"]));
					}
				}
			}

			return cajaInvetario;
		}
	}
}
