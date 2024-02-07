using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioTransactionXmlCT2 : IRepositorioTransactionXmlCT2
	{
		private readonly DBHelper _dbHelper;

		public RepositorioTransactionXmlCT2(DBHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}

		public async Task<TransactionXmlCT2> Obtener(string cadenaConexion)
		{
			_dbHelper.CadenaConexion = cadenaConexion;
			SqlParameter[] dbParams = null;
			var dr = await _dbHelper.ExecuteReaderText("SELECT CONVERT(char(12), insertdate, 113) FechaFormato, COUNT(*) Registros " +
				"FROM dbo.TransactionXmlCT2 WHERE CONVERT(char, insertdate, 111) = CONVERT(char, GETDATE(), 111) " +
				"GROUP BY CONVERT(char(12), insertdate, 113)", dbParams);
			TransactionXmlCT2 transactionXmlCT2 = new TransactionXmlCT2();

			while (await dr.ReadAsync())
			{
				transactionXmlCT2 = new TransactionXmlCT2
				{
					Cantidad = Convert.ToInt32(dr["Registros"]),
					FechaFormato = dr["FechaFormato"].ToString(),
				};
			}

			dr.Close();
			return transactionXmlCT2;
		}


	}
}
