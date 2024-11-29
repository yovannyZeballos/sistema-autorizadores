using Npgsql;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMaeCaja : RepositorioGenerico<SGPContexto, Mae_Caja>, IRepositorioMaeCaja
    {
        /// <summary>
		/// Inicializa una nueva instancia de la clase <see cref="RepositorioMaeCaja"/>.
		/// </summary>
		/// <param name="context">El contexto de la base de datos.</param>
		public RepositorioMaeCaja(SGPContexto context) : base(context) { }

        /// <summary>
        /// Obtiene el contexto de la base de datos.
        /// </summary>
        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }

        /// <summary>
        /// Obtiene una lista de cajas mediante un query SQL.
        /// </summary>
        /// <returns>Lista de cajas con sus datos correspondientes.</returns>
        public async Task<DataTable> ObtenerCajasPorEmpresaAsync(string codEmpresa)
        {
            var query = @"
            SELECT 
                mc.""COD_EMPRESA"" || ' ' || me.""NOM_EMPRESA"" as ""EMPRESA"",
	            mc.""COD_CADENA"" || ' ' || mc.""NOM_CADENA"" as ""CADENA"",
	            ml.""COD_REGION"" || ' ' || mr.""NOM_REGION"" as ""REGION"",
	            mz.""COD_ZONA"" || ' ' || mz.""NOM_ZONA"" as ""ZONA"",
	            ml.""COD_LOCAL"" || ' ' || ml.""NOM_LOCAL"" ""LOCAL"",
                mlc.""NUM_CAJA"",
                mlc.""IP_ADDRESS"",
                CASE ml.""TIP_ESTADO""
                    WHEN 'A' THEN 'ACTIVO'
                    WHEN 'E' THEN 'ELIMINADO'
                    WHEN 'P' THEN 'POR APERTURAR'
                END AS ""ESTADO_LOCAL"",
                CASE mlc.""TIP_ESTADO""
                    WHEN 'A' THEN 'ACTIVO'
                    WHEN 'E' THEN 'ELIMINADO'
                END AS ""ESTADO_CAJA"",
                mlc.""TIP_UBICACION"",
                mlc.""TIP_CAJA"" 
            FROM
                ""SGP"".""MAE_CADENA"" mc
            INNER JOIN
                ""SGP"".""MAE_EMPRESA"" me
                ON mc.""COD_EMPRESA"" = me.""COD_EMPRESA""
            INNER JOIN
                ""SGP"".""MAE_LOCAL"" ml
                ON mc.""COD_EMPRESA"" = ml.""COD_EMPRESA""
                AND mc.""COD_CADENA"" = ml.""COD_CADENA""
            INNER JOIN
                ""SGP"".""MAE_LOCAL_CAJA"" mlc
                ON ml.""COD_EMPRESA"" = mlc.""COD_EMPRESA""
                AND ml.""COD_CADENA"" = mlc.""COD_CADENA""
                AND ml.""COD_LOCAL"" = mlc.""COD_LOCAL""
            INNER JOIN
                ""SGP"".""MAE_REGION"" mr
                ON ml.""COD_EMPRESA"" = mr.""COD_EMPRESA""
                AND ml.""COD_CADENA"" = mr.""COD_CADENA""
                AND ml.""COD_REGION"" = mr.""COD_REGION""
            INNER JOIN
                ""SGP"".""MAE_ZONA"" mz
                ON ml.""COD_EMPRESA"" = mz.""COD_EMPRESA""
                AND ml.""COD_CADENA"" = mz.""COD_CADENA""
                AND ml.""COD_REGION"" = mz.""COD_REGION""
                AND ml.""COD_ZONA"" = mz.""COD_ZONA""
            WHERE 
                    ml.""COD_EMPRESA"" = @CodEmpresa
            ORDER BY
                mc.""COD_EMPRESA"",
                mc.""COD_CADENA"",
                ml.""COD_LOCAL"",
                mlc.""NUM_CAJA"";
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
