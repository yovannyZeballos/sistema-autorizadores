using Oracle.ManagedDataAccess.Client;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System.Collections.Generic;
using System.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioAutorizadores : CadenasConexion, IRepositorioAutorizadores
    {
        private readonly int _commandTimeout;

        public RepositorioAutorizadores()
        {
            _commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
        }

        public async Task Crear(Autorizador autorizador)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_REGISTRA_SUP_MANUAL", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout;

                await command.Connection.OpenAsync();
                command.Parameters.Add("PINVC_CODEMP", OracleDbType.Varchar2, autorizador.Codigo, ParameterDirection.Input);
                command.Parameters.Add("PINVC_NOMEMP", OracleDbType.Varchar2, autorizador.Nombres, ParameterDirection.Input);
                command.Parameters.Add("PINVC_APEPAT", OracleDbType.Varchar2, autorizador.ApellidoPaterno, ParameterDirection.Input);
                command.Parameters.Add("PINVC_APEMAT", OracleDbType.Varchar2, autorizador.ApellidoMaterno, ParameterDirection.Input);
                command.Parameters.Add("PINVC_NUMDOC", OracleDbType.Varchar2, autorizador.NumeroDocumento, ParameterDirection.Input);
                command.Parameters.Add("PINVC_ESTEMP", OracleDbType.Varchar2, autorizador.Estado, ParameterDirection.Input);
                command.Parameters.Add("PINNU_LOCAL", OracleDbType.Decimal, autorizador.CodLocal, ParameterDirection.Input);
                command.Parameters.Add("PINVC_USRCRE", OracleDbType.Varchar2, autorizador.UsuarioCreacion, ParameterDirection.Input);
                command.Parameters.Add("PINNU_ERROR", OracleDbType.Decimal, 1, ParameterDirection.Output);
                command.Parameters.Add("PINVC_MSGERR", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

                await command.ExecuteNonQueryAsync();

                var error = Convert.ToDecimal(command.Parameters["PINNU_ERROR"].Value.ToString());
                var mensjaeError = command.Parameters["PINVC_MSGERR"].Value.ToString();

                if (error == -1)
                    throw new Exception(mensjaeError);

                connection.Close();
                connection.Dispose();
            }
        }

        public async Task<string> GenerarArchivo(string tipoSO)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_GENERA_ARCHIVO_TIPO", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();

                command.Parameters.Add("vTIPO_SO", OracleDbType.Varchar2, tipoSO, ParameterDirection.Input);
                command.Parameters.Add("resultado", OracleDbType.Varchar2, 500, "", ParameterDirection.Output);

                await command.ExecuteNonQueryAsync();

                var resultado = command.Parameters["resultado"].Value.ToString();

                connection.Close();
                connection.Dispose();

                return resultado;

            }


        }

        public async Task<DataTable> ListarColaboradores(string codigoLocal, string codigoEmpresa)
        {
            var colaboradores = new List<Colaborador>();
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_MANT_LISTA_COLA_OFISIS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync(); 
                command.Parameters.Add("vCOD_EMPR", OracleDbType.Varchar2, codigoEmpresa, ParameterDirection.Input);
                command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codigoLocal, ParameterDirection.Input);
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);


                connection.Close();
                connection.Dispose();

                return datatable;

            }

        }

        public async Task<DataTable> ListarAutorizador(string codigoLocal)
        {
            var colaboradores = new List<Autorizador>();
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_MANT_LISTA_COLA_AUTORIZADOS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("vCOD_LOCAL", OracleDbType.Varchar2, codigoLocal, ParameterDirection.Input);
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);


                connection.Close();
                connection.Dispose();

                return datatable;

                //if (dr != null && dr.HasRows)
                //{
                //    while (await dr.ReadAsync())
                //    {
                //        colaboradores.Add(new Autorizador
                //        (
                //            dr["emp_codigo"].ToString(),
                //            dr["emp_nombre"].ToString(),
                //            dr["emp_apepat"].ToString(),
                //            dr["emp_apemat"].ToString(),
                //            dr["emp_numdoc"].ToString(),
                //            dr["emp_estado"].ToString(),
                //            Convert.ToInt32(dr["loc_numero"]),
                //            dr["EMP_USRCREA"].ToString(),
                //            dr["AUT_CODAUTORIZA"].ToString(),
                //            Convert.ToDateTime(dr["EMP_FECCREA"]).ToString("dd/MM/yyyy"),
                //            dr["tar_numtar"].ToString(),
                //            dr["TAR_ESTIMP"].ToString() == "A" ? "S" : "N"
                //        ));
                //    }
                //}

                //connection.Close();
                //connection.Dispose();

                //return colaboradores;
            }

        }

        public async Task ActualizarEstadoArchivo(Autorizador autorizador)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_CAMBIO_GENERACION_ARCHIVO", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout;

                await command.Connection.OpenAsync();
                command.Parameters.Add("PAUT_CODAUTORIZA", OracleDbType.Varchar2, autorizador.CodigoAutorizador, ParameterDirection.Input);
                command.Parameters.Add("PTAR_NUMTAR", OracleDbType.Varchar2, autorizador.NumeroTarjeta, ParameterDirection.Input);
                command.Parameters.Add("PINNU_LOCAL", OracleDbType.Varchar2, autorizador.CodLocal, ParameterDirection.Input);
                command.Parameters.Add("PEMP_CODIGO", OracleDbType.Varchar2, autorizador.Codigo, ParameterDirection.Input);
                command.Parameters.Add("PINNU_ERROR", OracleDbType.Decimal, 1, ParameterDirection.Output);
                command.Parameters.Add("PINVC_MSGERR", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

                await command.ExecuteNonQueryAsync();

                var error = Convert.ToDecimal(command.Parameters["PINNU_ERROR"].Value.ToString());
                var mensjaeError = command.Parameters["PINVC_MSGERR"].Value.ToString();

                if (error == -1)
                    throw new Exception(mensjaeError);

                connection.Close();
                connection.Dispose();
            }
        }

        public async Task Eliminar(Autorizador autorizador)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUTORIZADOR.SP_ELIMINAR_AUTORIZADOR", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout;

                await command.Connection.OpenAsync();
                command.Parameters.Add("PAUT_CODAUTORIZA", OracleDbType.Varchar2, autorizador.CodigoAutorizador, ParameterDirection.Input);
                command.Parameters.Add("PEMP_CODIGO", OracleDbType.Varchar2, autorizador.Codigo, ParameterDirection.Input);
                command.Parameters.Add("PAUT_USRMOD", OracleDbType.Varchar2, autorizador.UsuarioCreacion, ParameterDirection.Input);
                command.Parameters.Add("PINNU_ERROR", OracleDbType.Decimal, 1, ParameterDirection.Output);
                command.Parameters.Add("PINVC_MSGERR", OracleDbType.Varchar2, 250, "", ParameterDirection.Output);

                await command.ExecuteNonQueryAsync();

                var error = Convert.ToDecimal(command.Parameters["PINNU_ERROR"].Value.ToString());
                var mensjaeError = command.Parameters["PINVC_MSGERR"].Value.ToString();

                if (error == -1)
                    throw new Exception(mensjaeError);

                connection.Close();
                connection.Dispose();
            }
        }

        public async Task<DataTable> ListarColaboradoresCesados()
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_G3_LISTA_ELIMINADOS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("p_RECORDSET", OracleDbType.RefCursor, 1, ParameterDirection.Output);

                var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                var datatable = new DataTable();
                datatable.Load(dr);


                connection.Close();
                connection.Dispose();

                return datatable;
            }
        }

        public async Task<DataTable> ListarColaboradoresMass(string codEmpresa)
        {
            using (var connection = new OracleConnection(CadenaConexionAutorizadores))
            {
                var command = new OracleCommand("PKG_ICT2_AUT_PROCESOS.SP_G6_LISTA_COLAB_MASS_SIN_AUT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = _commandTimeout
                };

                await command.Connection.OpenAsync();
                command.Parameters.Add("VCOD_EMPRESA", OracleDbType.Varchar2, codEmpresa, ParameterDirection.Input);
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
