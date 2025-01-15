using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioMaeLocal : RepositorioGenerico<SGPContexto, Mae_Local>, IRepositorioMaeLocal
	{
		public RepositorioMaeLocal(SGPContexto context) : base(context) { }

		public SGPContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}

        /// <summary>
        /// Obtiene una lista de locales por empresa mediante un query SQL.
        /// </summary>
        /// <returns>Lista de cajas con sus datos correspondientes.</returns>
        public async Task<DataTable> ObtenerLocalesPorEmpresaAsync(string codEmpresa)
        {
            var query = @"
            SELECT
				mc.""COD_EMPRESA"" || ' ' || me.""NOM_EMPRESA"" as ""EMPRESA"",
				mc.""COD_CADENA"" || ' ' || mc.""NOM_CADENA"" as ""CADENA"",
				ml.""COD_REGION"" || ' ' || mr.""NOM_REGION"" as ""REGION"",
				mz.""COD_ZONA"" || ' ' || mz.""NOM_ZONA"" as ""ZONA"",
				ml.""COD_LOCAL"",
				ml.""NOM_LOCAL"",
				ml.""DIR_LOCAL"",
				ml.""UBIGEO"",
				CASE ml.""TIP_ESTADO""
					WHEN 'A' THEN 'ACTIVO'
					WHEN 'I' THEN 'INACTIVO'
					WHEN 'E' THEN 'ELIMINADO'
					WHEN 'D' THEN 'DESESTIMADO'
				END AS ""ESTADO_LOCAL"",
				ml.""TIP_LOCAL"",
				ml.""IP"",
				ml.""FEC_ESTIMADA"" ,
				ml.""FEC_APERTURA"",
				ml.""FEC_CIERRE""
			FROM
				""SGP"".""MAE_LOCAL"" ml
			INNER JOIN
				""SGP"".""MAE_ZONA"" mz
				on
				ml.""COD_EMPRESA"" = mz.""COD_EMPRESA""
				and ml.""COD_CADENA"" = mz.""COD_CADENA""
				and ml.""COD_REGION"" = mz.""COD_REGION""
				and ml.""COD_ZONA"" = mz.""COD_ZONA""
			INNER JOIN
				""SGP"".""MAE_REGION"" mr
				on
				ml.""COD_EMPRESA"" = mr.""COD_EMPRESA""
				and ml.""COD_CADENA"" = mr.""COD_CADENA""
				and ml.""COD_REGION"" = mr.""COD_REGION""
			INNER JOIN
				""SGP"".""MAE_CADENA"" mc
				on
				ml.""COD_EMPRESA"" = mc.""COD_EMPRESA""
				and ml.""COD_CADENA"" = mc.""COD_CADENA""
			INNER JOIN
				""SGP"".""MAE_EMPRESA"" me
				on
				mc.""COD_EMPRESA"" = me.""COD_EMPRESA""
			WHERE ml.""COD_EMPRESA"" = @CodEmpresa
			ORDER BY
				ml.""COD_EMPRESA"",
				ml.""COD_CADENA"",
				ml.""COD_REGION"",
				ml.""COD_ZONA"",
				ml.""COD_LOCAL"";
            ";

            var resultado = new List<Mae_Caja>();

            var dataTable = new DataTable();

            using (var connection = new NpgsqlConnection(AppDBMyBDContext.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CodEmpresa", codEmpresa);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }
    }
}
