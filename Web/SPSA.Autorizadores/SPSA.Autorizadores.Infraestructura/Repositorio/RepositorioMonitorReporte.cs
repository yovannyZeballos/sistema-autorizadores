using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMonitorReporte : CadenasConexion, IRepositorioMonitorReporte
    {
        private readonly int _commandTimeout;

        public RepositorioMonitorReporte()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task Crear(MonitorReporte localMonitor)
        {
            using (var connection = new SqlConnection(CadenaConexionCarteleria))
            {
                var command = new SqlCommand("SP_MONI_INSERTAR_LOCAL_MONITOR", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 10,
                    Value = localMonitor.CodEmpresa,
                    ParameterName = "@COD_EMPRESA"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 10,
                    Value = localMonitor.Formato,
                    ParameterName = "@COD_FORMATO"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 10,
                    Value = localMonitor.CodLocal,
                    ParameterName = "@COD_LOCAL"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = localMonitor.FechaProceso,
                    ParameterName = "@FEC_PROCESO"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = localMonitor.FechaCierre,
                    ParameterName = "@FEC_CIERRE"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 10,
                    Value = localMonitor.HoraInicio,
                    ParameterName = "@HORA_INICIO"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 10,
                    Value = localMonitor.HoraFin,
                    ParameterName = "@HORA_FIN"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 1,
                    Value = localMonitor.Estado,
                    ParameterName = "@TIP_ESTADO"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 250,
                    Value = localMonitor.Observacion ?? "",
                    ParameterName = "@DES_OBS"
                });

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<DataTable> ListarMonitorReporte(string codEmpresa, DateTime fecha, string estado)
        {
            using (var connection = new SqlConnection(CadenaConexionCarteleria))
            {
                var command = new SqlCommand("SP_MONI_LISTAR_LOCAL_MONITOR", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Size = 10,
                    Value = codEmpresa ?? "",
                    ParameterName = "@COD_EMPRESA"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = fecha,
                    ParameterName = "@FEC_CIERRE"
                });

                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = estado,
                    Size = 1,
                    ParameterName = "@TIP_ESTADO"
                });

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
