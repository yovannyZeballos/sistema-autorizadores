using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioInventarioTipo : CadenasConexion, IRepositorioInventarioTipo
	{
		private readonly DBHelper _dbHelper;

		public RepositorioInventarioTipo(DBHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}

		public async Task<List<InventarioTipo>> Listar(string codigo)
		{
			var lista = new List<InventarioTipo>();

			_dbHelper.CadenaConexion = CadenaConexionCarteleria;

			SqlParameter[] dbParams = new SqlParameter[]
			{
				_dbHelper.MakeParam("@COD_CARACT",codigo,SqlDbType.VarChar,ParameterDirection.Input,3),
			};

			using (var dr = await _dbHelper.ExecuteReader("SP_LOC_SERV_INV_LISTAR_TIPO", dbParams))
			{
				if (dr != null && dr.HasRows)
				{
					while (await dr.ReadAsync())
					{
						lista.Add(new InventarioTipo
						{
							Codigo = dr["COD_TIPO"].ToString(),
							Nombre = dr["NOM_TIPO"].ToString()
						});
					}
				}
			}
			return lista;
		}
	}
}
