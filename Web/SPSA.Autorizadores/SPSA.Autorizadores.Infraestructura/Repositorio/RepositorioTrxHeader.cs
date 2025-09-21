using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SPSA.Autorizadores.Dominio.Contrato.Dto;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Utiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
	public class RepositorioTrxHeader : CadenasConexion, IRepositorioTrxHeader
	{
		private readonly int _commandTimeout;

		public RepositorioTrxHeader()
		{
			_commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
		}

		public async Task<List<DocumentoElectronico>> ListarDocumentosElectronicos(ListarDocumentoElectronicoDto documentoElectronico)
		{
			List<DocumentoElectronico> documentos = new List<DocumentoElectronico>();

			string query = @"
                    SELECT * FROM sgp.fn_Consulta_Head_trx(@p_local, @p_fecha_inicio, @p_fecha_fin, @p_tipoDoc, @p_rutdoc, @p_tipodoc_cli, @p_cajero, @p_caja, @p_numtrx, @p_numero_pag, @p_tamano_pag)";

			using (var connection = new NpgsqlConnection(CadenaConexionCT3_SPSA_SGP))
			using (var command = new NpgsqlCommand(query, connection))
			{
                command.Parameters.Add(new NpgsqlParameter("@p_local", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = string.IsNullOrWhiteSpace(documentoElectronico.Codlocal) ? 0 : Convert.ToDecimal(documentoElectronico.Codlocal) });
                command.Parameters.Add(new NpgsqlParameter("@p_fecha_inicio", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = documentoElectronico.FechaInicio });
                command.Parameters.Add(new NpgsqlParameter("@p_fecha_fin", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = documentoElectronico.FechaFin });
                command.Parameters.Add(new NpgsqlParameter("@p_tipoDoc", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = documentoElectronico.TipoDocumento });
                command.Parameters.Add(new NpgsqlParameter("@p_rutdoc", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = documentoElectronico.NroDocCliente ?? "" });
                command.Parameters.Add(new NpgsqlParameter("@p_tipodoc_cli", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = documentoElectronico.TipoDocCliente });
                command.Parameters.Add(new NpgsqlParameter("@p_cajero", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = string.IsNullOrWhiteSpace(documentoElectronico.Cajero) ? "0" : documentoElectronico.Cajero });
                command.Parameters.Add(new NpgsqlParameter("@p_caja", NpgsqlTypes.NpgsqlDbType.Smallint) { Value = string.IsNullOrWhiteSpace(documentoElectronico.Caja) ? 0 : Convert.ToInt16(documentoElectronico.Caja) });
                command.Parameters.Add(new NpgsqlParameter("@p_numtrx", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = string.IsNullOrWhiteSpace(documentoElectronico.NroTransaccion) ? 0 : Convert.ToInt64(documentoElectronico.NroTransaccion) });
                command.Parameters.Add(new NpgsqlParameter("@p_numero_pag", NpgsqlTypes.NpgsqlDbType.Integer) { Value = documentoElectronico.NumeroPagina });
                command.Parameters.Add(new NpgsqlParameter("@p_tamano_pag", NpgsqlTypes.NpgsqlDbType.Integer) { Value = documentoElectronico.TamañoPagina });

                await connection.OpenAsync();
				using (var dr = await command.ExecuteReaderAsync())
				{
					if (dr != null && dr.HasRows)
					{
						while (await dr.ReadAsync())
						{
							documentos.Add(new DocumentoElectronico
							{
								Local = dr["local"] != DBNull.Value ? dr["local"].ToString() : "",
								Caja = dr["caja"] != DBNull.Value ? dr["caja"].ToString() : "",
								NroTransaccion = dr["numero_transaccion"] != DBNull.Value ? dr["numero_transaccion"].ToString() : null,
								Fecha = dr["fecha"] != DBNull.Value ? dr["fecha"].ToString() : "",
								Importe = dr["importe"] != DBNull.Value ? Convert.ToDecimal(dr["importe"]) : 0,
								TipoDocElectronico = dr["tipo_documento_Electronico"] != DBNull.Value ? dr["tipo_documento_Electronico"].ToString() : "",
								DocElectronico = dr["nro_documento_Electronico"] != DBNull.Value ? dr["nro_documento_Electronico"].ToString() : "",
								MedioPago = dr["medio_pago"] != DBNull.Value ? dr["medio_pago"].ToString() : "",
								Cajero = dr["cajero"] != DBNull.Value ? dr["cajero"].ToString() : "",
								TipoDocumento = dr["tipo_doc_cliente"] != DBNull.Value ? dr["tipo_doc_cliente"].ToString() : "",
								NroDocumento = dr["nro_doc_cliente"] != DBNull.Value ? dr["nro_doc_cliente"].ToString() : "",
								TotalRegistros = dr["total_registros"] != DBNull.Value ? Convert.ToInt32(dr["total_registros"]) : 0
							});
						}
					}
					return documentos;
				}
			}
		}

		public async Task<(int cantidadTransacciones, decimal montoFinal)> ObtenerCantidadTransacciones(int local, string fecha)
		{
            using (var connection = new OracleConnection(CadenaConexionBCT))
            using (var command = new OracleCommand("ADM_SPSA.SF_MONITOR_BCT_TRXS_XLOCFCH", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout;

                var returnParam = new OracleParameter("RETURN_VALUE", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnParam);

                command.Parameters.Add("V_FECHA", OracleDbType.Varchar2).Value = fecha;
                command.Parameters.Add("N_SUCURSAL", OracleDbType.Int32).Value = local;

                var cantTrxParam = new OracleParameter("NO_CANT_TRX", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cantTrxParam);

                var impVtaParam = new OracleParameter("NO_IMP_VTA", OracleDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(impVtaParam);

                var sqlCodeParam = new OracleParameter("NO_SQL_CODE", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(sqlCodeParam);

                var errorParam = new OracleParameter("VO_ERROR", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(errorParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                int cantidad = ((OracleDecimal)cantTrxParam.Value).ToInt32();
                decimal monto = ((OracleDecimal)impVtaParam.Value).Value;
                int returnCode = ((OracleDecimal)returnParam.Value).ToInt32();
                int errorCode = ((OracleDecimal)sqlCodeParam.Value).ToInt32();
                string errorMsg = errorParam.Value?.ToString();

                return (cantidad, monto);
            }

        }
	}
}
