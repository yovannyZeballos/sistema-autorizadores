using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioReportes : CadenasConexion, IRepositorioReportes
    {
        private readonly int _commandTimeout;

        public RepositorioReportes()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task<DataTable> ListarLocalesCambioPrecio(string codLocal, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                var command = new OracleCommand("PKG_CT2_SCTRX.SP_CONSULTA_CAMBPRECIO_CRB", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add("PINVC_FECINI", OracleDbType.Varchar2, fechaInicio.ToString("yyyyMMdd"), ParameterDirection.Input);
                command.Parameters.Add("PINVC_FECFIN", OracleDbType.Varchar2, fechaFin.ToString("yyyyMMdd"), ParameterDirection.Input);
                command.Parameters.Add("PINNU_LOCAL", OracleDbType.Int32, Convert.ToInt32(codLocal), ParameterDirection.Input);
                command.Parameters.Add("IO_CURSOR", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);

                connection.Close();
                connection.Dispose();

                return datatable;
            }
        }

        public async  Task<DataTable> ListarLocalesNotaCredito(string codLocal, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var connection = new OracleConnection(CadenaConexionCT2))
            {
                var command = new OracleCommand("PKG_CT2_SCTRX.SP_CONSULTA_NOTACREDITO", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add("PINVC_FECINI", OracleDbType.Varchar2, fechaInicio.ToString("yyyyMMdd"), ParameterDirection.Input);
                command.Parameters.Add("PINVC_FECFIN", OracleDbType.Varchar2, fechaFin.ToString("yyyyMMdd"), ParameterDirection.Input);
                command.Parameters.Add("PINNU_LOCAL", OracleDbType.Int32, Convert.ToInt32(codLocal), ParameterDirection.Input);
                command.Parameters.Add("IO_CURSOR", OracleDbType.RefCursor, 1, ParameterDirection.Output);

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
