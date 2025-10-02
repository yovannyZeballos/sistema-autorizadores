using Npgsql;
using System.Data;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioInvCajas : RepositorioGenerico<SGPContexto, InvCajas>, IRepositorioInvCajas
	{
        public RepositorioInvCajas(SGPContexto context) : base(context) { }

		public SGPContexto AppDBMyBDContext
		{
			get { return _contexto; }
		}

        public async Task<DataTable> DescargarInventarioCajas(string codEmpresa)
        {
            var query = @"SELECT * 
              FROM ""SGP"".""v_inventario_caja"" 
              WHERE cod_empresa = @CodEmpresa";

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
