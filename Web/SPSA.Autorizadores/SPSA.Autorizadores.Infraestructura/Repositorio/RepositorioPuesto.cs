using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioPuesto : CadenasConexion, IRepositorioPuesto
    {
        private readonly int _commandTimeout;

        public RepositorioPuesto()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task Actualizar(Puesto puesto)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_UPD_PUESTO_SEL", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("VCO_EMPR", OracleDbType.Varchar2, puesto.CodEmpresa, ParameterDirection.Input);
                command.Parameters.Add("VCO_PUES_TRAB", OracleDbType.Varchar2, puesto.CodPuesto, ParameterDirection.Input);
                command.Parameters.Add("VCK_SELEC", OracleDbType.Varchar2, puesto.Select, ParameterDirection.Input);
                command.Parameters.Add("VRPTA", OracleDbType.Decimal, 1, ParameterDirection.Output);
                command.Parameters.Add("VMSJ", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

                await command.ExecuteNonQueryAsync();

                var error = Convert.ToDecimal(command.Parameters["VRPTA"].Value.ToString());
                var mensjaeError = command.Parameters["VMSJ"].Value.ToString();

                if (error == -1)
                    throw new Exception(mensjaeError);

                connection.Close();
                connection.Dispose();
            }
        }

        public async Task<DataTable> Listar(string codEmpresa)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_G5_LISTA_PUESTO", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("VCO_EMPR", OracleDbType.Varchar2, codEmpresa, ParameterDirection.Input);
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);


                connection.Close();
                connection.Dispose();

                return datatable;
            }
        }
    }
}
