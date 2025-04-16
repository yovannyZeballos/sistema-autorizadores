using Npgsql;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioSolicitudUsuarioASR : RepositorioGenerico<SGPContexto, ASR_SolicitudUsuario>, IRepositorioSolicitudUsuarioASR
    {
        public RepositorioSolicitudUsuarioASR(SGPContexto context) : base(context) { }

        public SGPContexto SGPContext
		{
            get { return _contexto; }
        }

		public async Task ActualizarMotivoRechazo(int numSolicitud, string motivo, string estado)
		{
			using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand(@"CALL ""SGP"".""SP_ASR_ACTUALIZAR_MOTIVO_RECHAZO""(@P_NUM_SOLICITUD, @P_MOTIVO, @P_IND_APROBADO)", connection))
				{
					command.Parameters.Add(new NpgsqlParameter("@P_NUM_SOLICITUD", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = numSolicitud });
					command.Parameters.Add(new NpgsqlParameter("@P_MOTIVO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = motivo });
					command.Parameters.Add(new NpgsqlParameter("@P_IND_APROBADO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = estado });

					await command.ExecuteNonQueryAsync();
				}
			}
		}

		public async Task AprobarSolicitud(ASR_Usuario usuario)
		{
			using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand(@"CALL ""SGP"".""SP_ASR_APROBAR_SOLICITUD""(@P_NUM_SOLICITUD, @P_IND_ACTIVO, @P_FLG_ENVIO, @P_FEC_ENVIO, @P_USU_AUTORIZA, @P_USU_CREACION, @P_USU_ELIMINA, @P_FEC_ELIMINA)", connection))
				{
					command.Parameters.Add(new NpgsqlParameter("@P_NUM_SOLICITUD", NpgsqlTypes.NpgsqlDbType.Integer) { Value = usuario.NumSolicitud });
					command.Parameters.Add(new NpgsqlParameter("@P_IND_ACTIVO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.IndActivo ?? DBNull.Value });
					command.Parameters.Add(new NpgsqlParameter("@P_FLG_ENVIO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.FlgEnvio ?? DBNull.Value });
					command.Parameters.Add(new NpgsqlParameter("@P_FEC_ENVIO", NpgsqlTypes.NpgsqlDbType.Date) { Value = (object)usuario.FecEnvio ?? DBNull.Value });
					command.Parameters.Add(new NpgsqlParameter("@P_USU_AUTORIZA", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuAutoriza ?? DBNull.Value });
					command.Parameters.Add(new NpgsqlParameter("@P_USU_CREACION", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuCreacion ?? DBNull.Value });
					command.Parameters.Add(new NpgsqlParameter("@P_USU_ELIMINA", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (object)usuario.UsuElimina ?? DBNull.Value });
					command.Parameters.Add(new NpgsqlParameter("@P_FEC_ELIMINA", NpgsqlTypes.NpgsqlDbType.Date) { Value = (object)usuario.FecElimina ?? DBNull.Value });

					await command.ExecuteNonQueryAsync();
				}
			}
		}

		public async Task<List<ASR_UsuarioListado>> ListarSolicitudes(string usuarioLogin, string tipoUsuario, string CodEmpresa,
			int numeroPagina, int tamañoPagina)
		{
			List<ASR_UsuarioListado> usuarios = new List<ASR_UsuarioListado>();

			using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
			{
				await connection.OpenAsync();

				string query = @"
                    SELECT * FROM ""SGP"".""SF_ASR_SOLICITUD_USUARIO_LISTAR""(@P_USUARIO_LOGIN, @P_TIP_USUARIO, @P_COD_EMPRESA, @P_PAGE_NUMBER, @P_PAGE_SIZE)";

				using (var command = new NpgsqlCommand(query, connection))
				{
					command.Parameters.Add(new NpgsqlParameter("@P_USUARIO_LOGIN", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuarioLogin });
					command.Parameters.Add(new NpgsqlParameter("@P_TIP_USUARIO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = tipoUsuario });
					command.Parameters.Add(new NpgsqlParameter("@P_COD_EMPRESA", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = CodEmpresa });
					command.Parameters.Add(new NpgsqlParameter("@P_PAGE_NUMBER", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numeroPagina });
					command.Parameters.Add(new NpgsqlParameter("@P_PAGE_SIZE", NpgsqlTypes.NpgsqlDbType.Integer) { Value = tamañoPagina });

					using (var dr = await command.ExecuteReaderAsync())
					{
						if (dr != null && dr.HasRows)
						{
							while (await dr.ReadAsync())
							{
								usuarios.Add(new ASR_UsuarioListado
								{
									NumSolicitud = Convert.ToInt32(dr["NUM_SOLICITUD"]),
									CodLocal = dr["COD_LOCAL"].ToString(),
									NomLocal = dr["NOM_LOCAL"].ToString(),
									CodColaborador = dr["COD_COLABORADOR"] != DBNull.Value ? dr["COD_COLABORADOR"].ToString() : string.Empty,
									NoApelPate = dr["NO_APEL_PATE"] != DBNull.Value ? dr["NO_APEL_PATE"].ToString() : string.Empty,
									NoApelMate = dr["NO_APEL_MATE"] != DBNull.Value ? dr["NO_APEL_MATE"].ToString() : string.Empty,
									NoTrab = dr["NO_TRAB"] != DBNull.Value ? dr["NO_TRAB"].ToString() : string.Empty,
									DePuesTrab = dr["DE_PUES_TRAB"] != DBNull.Value ? dr["DE_PUES_TRAB"].ToString() : string.Empty,
									IndAprobado = dr["IND_APROBADO"] != DBNull.Value ? dr["IND_APROBADO"].ToString() : string.Empty,
									FecSolicita = dr["FEC_SOLICITA"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FEC_SOLICITA"]) : null,
									NumDocumentoIdentidad = dr["NU_DOCU_IDEN"] != DBNull.Value ? dr["NU_DOCU_IDEN"].ToString() : string.Empty,
									TotalRegistros = dr["TOTAL_REGISTROS"] != DBNull.Value ? Convert.ToInt32(dr["TOTAL_REGISTROS"]) : 0
								});
							}
						}
						return usuarios;
					}
				}
			}
		}

		public async Task<List<ASR_UsuarioListado>> ListarUsuarios(string usuarioLogin, string codLocal, 
            string usuAprobacion, string tipAccion, string indAprobado, string CodEmpresa,
			int numeroPagina, int tamañoPagina)
		{
            List<ASR_UsuarioListado> usuarios = new List<ASR_UsuarioListado>();

			using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
				await connection.OpenAsync();

				string query = @"
                    SELECT * FROM ""SGP"".""SF_ASR_USUARIO_LISTAR""(@P_USUARIO_LOGIN, @P_COD_LOCAL, @P_USU_APROBACION, @P_TIP_ACCION, @P_IND_APROBADO, @P_COD_EMPRESA, @P_PAGE_NUMBER, @P_PAGE_SIZE)";

				using (var command = new NpgsqlCommand(query, connection))
				{
					command.Parameters.Add(new NpgsqlParameter("@P_USUARIO_LOGIN", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuarioLogin });
					command.Parameters.Add(new NpgsqlParameter("@P_COD_LOCAL", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codLocal });
					command.Parameters.Add(new NpgsqlParameter("@P_USU_APROBACION", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuAprobacion });
					command.Parameters.Add(new NpgsqlParameter("@P_TIP_ACCION", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = tipAccion });
					command.Parameters.Add(new NpgsqlParameter("@P_IND_APROBADO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = indAprobado });
					command.Parameters.Add(new NpgsqlParameter("@P_COD_EMPRESA", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = CodEmpresa });
					command.Parameters.Add(new NpgsqlParameter("@P_PAGE_NUMBER", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numeroPagina });
					command.Parameters.Add(new NpgsqlParameter("@P_PAGE_SIZE", NpgsqlTypes.NpgsqlDbType.Integer) { Value = tamañoPagina });

					using (var dr = await command.ExecuteReaderAsync())
					{
						if (dr != null && dr.HasRows)
						{
							while (await dr.ReadAsync())
							{
								usuarios.Add(new ASR_UsuarioListado
								{
									NumSolicitud = Convert.ToInt32(dr["NUM_SOLICITUD"]),
									CodLocal = dr["COD_LOCAL"].ToString(),
									NomLocal = dr["NOM_LOCAL"].ToString(),
									CodColaborador = dr["COD_COLABORADOR"] != DBNull.Value ? dr["COD_COLABORADOR"].ToString() : string.Empty,
									CodUsuarioAsr = dr["COD_USUARIO_ASR"] != DBNull.Value ? dr["COD_USUARIO_ASR"].ToString() : string.Empty,
									NoApelPate = dr["NO_APEL_PATE"] != DBNull.Value ? dr["NO_APEL_PATE"].ToString() : string.Empty,
									NoApelMate = dr["NO_APEL_MATE"] != DBNull.Value ? dr["NO_APEL_MATE"].ToString() : string.Empty,
									NoTrab = dr["NO_TRAB"] != DBNull.Value ? dr["NO_TRAB"].ToString() : string.Empty,
									TipAccion = dr["TIP_ACCION"] != DBNull.Value ? dr["TIP_ACCION"].ToString() : string.Empty,
									TipUsuario = dr["TIP_USUARIO"] != DBNull.Value ? dr["TIP_USUARIO"].ToString() : string.Empty,
									TipColaborador = dr["TIP_COLABORADOR"] != DBNull.Value ? dr["TIP_COLABORADOR"].ToString() : string.Empty,
									DePuesTrab = dr["DE_PUES_TRAB"] != DBNull.Value ? dr["DE_PUES_TRAB"].ToString() : string.Empty,
									IndAprobado = dr["IND_APROBADO"] != DBNull.Value ? dr["IND_APROBADO"].ToString() : string.Empty,
									UsuSolicita = dr["USU_SOLICITA"] != DBNull.Value ? dr["USU_SOLICITA"].ToString() : string.Empty,
									UsuSolNoApelPate = dr["USU_SOL_NO_APEL_PATE"] != DBNull.Value ? dr["USU_SOL_NO_APEL_PATE"].ToString() : string.Empty,
									UsuSolNoApelMate = dr["USU_SOL_NO_APEL_MATE"] != DBNull.Value ? dr["USU_SOL_NO_APEL_MATE"].ToString() : string.Empty,
									UsuSolNoTrab = dr["USU_SOL_NO_TRAB"] != DBNull.Value ? dr["USU_SOL_NO_TRAB"].ToString() : string.Empty,
									FecSolicita = dr["FEC_SOLICITA"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FEC_SOLICITA"]) : null,
									UsuAprobacion = dr["USU_APROBACION"] != DBNull.Value ? dr["USU_APROBACION"].ToString() : string.Empty,
									UsuAprNoApelPate = dr["USU_APR_NO_APEL_PATE"] != DBNull.Value ? dr["USU_APR_NO_APEL_PATE"].ToString() : string.Empty,
									UsuAprNoApelMate = dr["USU_APR_NO_APEL_MATE"] != DBNull.Value ? dr["USU_APR_NO_APEL_MATE"].ToString() : string.Empty,
									UsuAprNoTrab = dr["USU_APR_NO_TRAB"] != DBNull.Value ? dr["USU_APR_NO_TRAB"].ToString() : string.Empty,
									FecAprobacion = dr["FEC_APROBACION"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["FEC_APROBACION"]) : null,
									Motivo = dr["MOTIVO"] != DBNull.Value ? dr["MOTIVO"].ToString() : string.Empty,
									TotalRegistros = dr["TOTAL_COUNT"] != DBNull.Value ? Convert.ToInt32(dr["TOTAL_COUNT"]) : 0
								});
							}
						}
						return usuarios;
					}
				}
			}
		}

		public async Task<List<ASR_UsuarioArchivo>> ListarArchivos()
		{
			List<ASR_UsuarioArchivo> archivos = new List<ASR_UsuarioArchivo>();

			using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
			{
				await connection.OpenAsync();

				string query = @"SELECT * FROM ""SGP"".""SF_ASR_GENERAR_ARCHIVO""()";

				using (var command = new NpgsqlCommand(query, connection))
				{
					using (var dr = await command.ExecuteReaderAsync())
					{
						if (dr != null && dr.HasRows)
						{
							while (await dr.ReadAsync())
							{
								archivos.Add(new ASR_UsuarioArchivo
								{
									NumSolicitud = Convert.ToInt16(dr["NUM_SOLICITUD"]),
									Contenido = dr["CONTENIDO"].ToString(),
									NombreArchivo = dr["NOMBRE_ARCHIVO"].ToString(),
								});
							}
						}
						return archivos;
					}
				}
			}
		}

		public async Task ActualizarFlagEnvio(long numSolicitud, string flagEnvio)
		{
			using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand(@"CALL ""SGP"".""SP_ASR_ACTUALIZAR_FLG_ENVIO""(@P_NUM_SOLICITUD, @P_FLG_ENVIO)", connection))
				{
					command.Parameters.Add(new NpgsqlParameter("@P_NUM_SOLICITUD", NpgsqlTypes.NpgsqlDbType.Integer) { Value = numSolicitud });
					command.Parameters.Add(new NpgsqlParameter("@P_FLG_ENVIO", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = flagEnvio });

					await command.ExecuteNonQueryAsync();
				}
			}
		}
	}
}
