using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioSovosLocal : CadenasConexion, IRepositorioSovosLocal
	{
		private readonly DBHelper _dbHelper;

		public RepositorioSovosLocal(DBHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}

		public async Task Crear(SovosLocal sovosLocal)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",sovosLocal.CodEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",sovosLocal.CodFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_LOCAL",sovosLocal.CodLocal,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@NOM_LOCAL",sovosLocal.NomLocal,SqlDbType.VarChar,ParameterDirection.Input,100),
				 _dbHelper.MakeParam("@IP_ADDRES",sovosLocal.Ip,SqlDbType.VarChar,ParameterDirection.Input,30),
				 _dbHelper.MakeParam("@IP_MASCARAS",sovosLocal.IpMascara,SqlDbType.VarChar,ParameterDirection.Input,30),
				 _dbHelper.MakeParam("@TIP_OS",sovosLocal.SO,SqlDbType.VarChar,ParameterDirection.Input,1),
				 _dbHelper.MakeParam("@GRUPO",sovosLocal.Grupo,SqlDbType.Decimal,ParameterDirection.Input),
				 _dbHelper.MakeParam("@TIP_ESTADO",sovosLocal.Estado,SqlDbType.Char,ParameterDirection.Input,1),
				 _dbHelper.MakeParam("@TIP_LOCAL",sovosLocal.TipoLocal,SqlDbType.Char,ParameterDirection.Input,1),
				 _dbHelper.MakeParam("@IND_FACTURA",sovosLocal.IndFactura,SqlDbType.Char,ParameterDirection.Input,1),
				 _dbHelper.MakeParam("@COD_LOCAL_SUNAT",sovosLocal.CodigoSunat,SqlDbType.VarChar,ParameterDirection.Input,4),
				 _dbHelper.MakeParam("@USER",sovosLocal.Usuario,SqlDbType.VarChar,ParameterDirection.Input,20),
				 _dbHelper.MakeParam("@FECHA",sovosLocal.Fecha,SqlDbType.DateTime,ParameterDirection.Input),
				 _dbHelper.MakeParam("@ERROR",0,SqlDbType.Int,ParameterDirection.Output),
				 _dbHelper.MakeParam("@MSGERR","",SqlDbType.VarChar,ParameterDirection.Output,250)
			};

			var outputs = await _dbHelper.ExecuteNonQueryWithOutput("SP_LOC_SAVE", dbParams);

			var error = Convert.ToInt32(outputs["@ERROR"].ToString());
			var mensajeError = outputs["@MSGERR"].ToString();

			if (error > 0)
				throw new Exception(mensajeError);

		}

		public async Task<List<SovosLocal>> ListarMonitor(string codEmpresa, DateTime fecha, int tipo)
		{
			var locales = new List<SovosLocal>();

			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@FEC_CIERRE",fecha,SqlDbType.DateTime,ParameterDirection.Input),
				 _dbHelper.MakeParam("@TIPO",tipo,SqlDbType.Int,ParameterDirection.Input),
			};

			var dr = await _dbHelper.ExecuteReader("SP_MONI_LISTAR_SOVOS_LOCAL", dbParams);

			if (dr != null && dr.HasRows)
			{
				while (await dr.ReadAsync())
				{
					locales.Add(new SovosLocal(dr["COD_EMPRESA"].ToString(), dr["COD_LOCAL"].ToString(), dr["COD_FORMATO"].ToString(), dr["IP_ADDRES"].ToString()));
				}
			}
			dr.Close();
			return locales;
		}

		public async Task<DataTable> ListarPorEmpresa(string codEmpresa, string codFormato)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",codFormato,SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var dr = await _dbHelper.ExecuteReader("SP_LOC_LISTA_XEMP", dbParams);
			var dt = new DataTable();
			dt.Load(dr);
			dr.Close();
			return dt;
		}

		public async Task<SovosLocal> ObtenerLocal(string codEmpresa, string codFormato, string codLocal)
		{
			SovosLocal sovosLocal = null;

			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",codFormato,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_LOCAL",codLocal,SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var dr = await _dbHelper.ExecuteReader("SP_LOC_FIND", dbParams);
			if (dr != null && dr.HasRows)
			{
				while (await dr.ReadAsync())
				{
					sovosLocal = new SovosLocal(codEmpresa, codLocal, codFormato, dr["NOM_LOCAL"].ToString(), dr["IP_ADDRES"].ToString(), dr["IP_MASCARAS"] is DBNull ? "" : dr["IP_MASCARAS"].ToString(),
						dr["TIP_OS"].ToString(), Convert.ToDecimal(dr["GRUPO"] is DBNull ? 0 : dr["GRUPO"]), dr["TIP_ESTADO"].ToString(), dr["TIP_LOCAL"].ToString(), dr["IND_FACTURA"].ToString(),
						dr["COD_LOCAL_SUNAT"].ToString(), null, null);
				}
			}

			return sovosLocal;
		}

		public async Task<DataTable> DescargarMaestro(string codEmpresa, string codFormato)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",codFormato,SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var dr = await _dbHelper.ExecuteReader("SP_LOC_DESCARGA_MAESTRO", dbParams);
			var dt = new DataTable();
			dt.Load(dr);
			dr.Close();
			return dt;
		}

		public async Task<List<SovosLocal>> ListarPorEmpresa(string codEmpresa)
		{
			_dbHelper.CadenaConexion = CadenaConexionCarteleria;
			var locales = new List<SovosLocal>();
			SqlParameter[] dbParams = new SqlParameter[]
			{
				 _dbHelper.MakeParam("@COD_EMPRESA",codEmpresa,SqlDbType.VarChar,ParameterDirection.Input,10),
				 _dbHelper.MakeParam("@COD_FORMATO",DBNull.Value,SqlDbType.VarChar,ParameterDirection.Input,10)
			};

			var dr = await _dbHelper.ExecuteReader("SP_LOC_LISTA_XEMP", dbParams);
			if (dr != null && dr.HasRows)
			{
				while (await dr.ReadAsync())
				{
					locales.Add(new SovosLocal(codEmpresa, dr["Cod. Local"].ToString(), dr["Nombre Local"].ToString()));
				}
			}

			return locales;
		}

	}
}
