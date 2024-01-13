using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.Globalization;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioTransactionXmlCT2 : CadenasConexion, IRepositorioTransactionXmlCT2
	{
		private readonly DBHelper _dbHelper;

		public RepositorioTransactionXmlCT2(DBHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}

		public async Task<(TransactionXmlCT2, List<TransactionXmlCT2>)> Obtener()
		{
			_dbHelper.CadenaConexion = CadenaConexionBCT;
			SqlParameter[] dbParams = null;
			var dr = await _dbHelper.ExecuteReader("SP_MONITOR_BCT_OBTENER_REGISTROS", dbParams);
			TransactionXmlCT2 transactionXmlCT2 = null;
			var listTransactionXmlCT2 = new List<TransactionXmlCT2>();

			while (await dr.ReadAsync())
			{
				transactionXmlCT2 = new TransactionXmlCT2
				{
					Cantidad = Convert.ToInt32(dr["Registros"]),
					FechaFormato = dr["FechaFormato"].ToString(),
				};
			}

			await dr.NextResultAsync();

			while (await dr.ReadAsync())
			{
				listTransactionXmlCT2.Add(new TransactionXmlCT2
				{
					Cantidad = Convert.ToInt32(dr["Registros"]),
					FechaFormato = dr["FechaFormato"].ToString(),
					Fecha = DateTime.ParseExact(dr["Fecha"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
				});
			}

			dr.Close();

			return (transactionXmlCT2, listTransactionXmlCT2);
		}
	}
}
