using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMaeHorario : RepositorioGenerico<SGPContexto, Mae_Horario>, IRepositorioMaeHorario
    {
		public RepositorioMaeHorario(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }

        public async Task<DataTable> ObtenerHorariosPorEmpresaAsync(string codEmpresa)
        {
            var query = @"";

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
