using Npgsql;
using SGP.Api.Models;
using SGP.Api.Services.BctService.DTOs;
using SGP.Api.Services.SgpService.DTOs;

namespace SGP.Api.Services.SgpService
{
    public class SgpService
    {
        private readonly string _conexionSGP;
        public SgpService(IConfiguration configuration)
        {
            _conexionSGP = configuration.GetConnectionString("SGP") ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'SGP' is null.");
        }

        #region MAE_LOCAL

        public List<EmpresaDto> ObtenerEmpresas()
        {
            var listaEmpresa = new List<EmpresaDto>();

            const string query = @"SELECT ""COD_EMPRESA"", ""NOM_EMPRESA"", ""COD_SOCIEDAD"", ""COD_EMPRESA_OFI"", ""RUC""
                                   FROM ""SGP"".""MAE_EMPRESA"";";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var empresa = new EmpresaDto
                    {
                        CodEmpresa = reader["COD_EMPRESA"]?.ToString(),
                        NomEmpresa = reader["NOM_EMPRESA"]?.ToString(),
                        CodSociedad = reader["COD_SOCIEDAD"]?.ToString(),
                        CodEmpresaOfi = reader["COD_EMPRESA_OFI"]?.ToString(),
                        Ruc = reader["RUC"]?.ToString()
                    };

                    listaEmpresa.Add(empresa);
                }

                return listaEmpresa;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener lista empresas::" + ex.Message, ex);
            }
        }

        public List<CadenaDto> ObtenerCadenas(string codEmpresa)
        {
            var listaCadenas = new List<CadenaDto>();

            const string query = @"SELECT ""COD_EMPRESA"", ""COD_CADENA"", ""NOM_CADENA"", ""CAD_NUMERO""
                                   FROM ""SGP"".""MAE_CADENA""
                                   WHERE ""COD_EMPRESA"" = :COD_EMPRESA;";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                command.Parameters.AddWithValue(":COD_EMPRESA", codEmpresa ?? (object)DBNull.Value);

                connection.Open();
                using var reader = command.ExecuteReader();

                var idxCadNumero = reader.GetOrdinal("CAD_NUMERO");

                while (reader.Read())
                {
                    var cadena = new CadenaDto
                    {
                        CodEmpresa = reader["COD_EMPRESA"]?.ToString(),
                        CodCadena = reader["COD_CADENA"]?.ToString(),
                        NomCadena = reader["NOM_CADENA"]?.ToString(),
                        CadNumero = reader.IsDBNull(idxCadNumero) ? 0 : reader.GetInt32(idxCadNumero)
                    };

                    listaCadenas.Add(cadena);
                }

                return listaCadenas;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener lista cadenas::" + ex.Message, ex);
            }
        }

        public LocalDto? ObtenerLocal(string codEmpresa, string codCadena, string codLocal)
        {
            LocalDto? local = null;

            const string query = @"SELECT ""COD_EMPRESA"", ""COD_CADENA"", ""COD_REGION"", ""COD_ZONA"", ""COD_LOCAL""
                                   FROM ""SGP"".""MAE_LOCAL""
                                   WHERE ""COD_EMPRESA"" = :COD_EMPRESA
                                     AND ""COD_CADENA""  = :COD_CADENA
                                     AND ""COD_LOCAL""   = :COD_LOCAL;";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                command.Parameters.AddWithValue(":COD_EMPRESA", codEmpresa ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(":COD_CADENA", codCadena ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(":COD_LOCAL", codLocal ?? (object)DBNull.Value);

                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    local = new LocalDto
                    {
                        CodEmpresa = reader["COD_EMPRESA"]?.ToString(),
                        CodCadena = reader["COD_CADENA"]?.ToString(),
                        CodRegion = reader["COD_REGION"]?.ToString(),
                        CodZona = reader["COD_ZONA"]?.ToString(),
                        CodLocal = reader["COD_LOCAL"]?.ToString()
                    };
                }

                return local;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener local::" + ex.Message, ex);
            }
        }
      
        public string CrearLocal(IRS_LOCALES local)
        {
            const string query = @"INSERT INTO ""SGP"".""MAE_LOCAL""
                                   (""COD_EMPRESA"", ""COD_CADENA"", ""COD_REGION"", ""COD_ZONA"", ""COD_LOCAL"", ""NOM_LOCAL"", ""TIP_ESTADO"",
                                    ""COD_LOCAL_PMM"", ""COD_LOCAL_OFIPLAN"", ""NOM_LOCAL_OFIPLAN"", ""COD_LOCAL_SUNAT"", ""TIP_LOCAL"",
                                    ""FEC_APERTURA"", ""USU_CREACION"", ""FEC_CREACION"")
                                   VALUES(:COD_EMPRESA, :COD_CADENA, :COD_REGION, :COD_ZONA, :COD_LOCAL, :NOM_LOCAL, :TIP_ESTADO,
                                          :COD_LOCAL_PMM, :COD_LOCAL_OFIPLAN, :NOM_LOCAL_OFIPLAN, :COD_LOCAL_SUNAT,
                                          :TIP_LOCAL, :FEC_APERTURA, :USU_CREACION, :FEC_CREACION);";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();

                if (string.IsNullOrWhiteSpace(local.LOC_NUMEROPMM))
                    local.LOC_NUMEROPMM = "0";

                command.Parameters.AddWithValue(":COD_EMPRESA", local.COD_EMPRESA ?? string.Empty);
                command.Parameters.AddWithValue(":COD_CADENA", local.COD_CADENA ?? string.Empty);
                command.Parameters.AddWithValue(":COD_REGION", "01");
                command.Parameters.AddWithValue(":COD_ZONA", local.COD_EMPRESA == "06" ? "00" : "01");
                command.Parameters.AddWithValue(":COD_LOCAL", local.LOC_NUMERO ?? string.Empty);
                command.Parameters.AddWithValue(":NOM_LOCAL", local.LOC_DESCRIPCION ?? string.Empty);
                command.Parameters.AddWithValue(":TIP_ESTADO", local.LOC_ACTIVO ?? string.Empty);
                command.Parameters.AddWithValue(":COD_LOCAL_PMM", int.TryParse(local.LOC_NUMEROPMM, out var pmm) ? pmm : 0);
                command.Parameters.AddWithValue(":COD_LOCAL_OFIPLAN", "0");
                command.Parameters.AddWithValue(":NOM_LOCAL_OFIPLAN", string.Empty);
                command.Parameters.AddWithValue(":COD_LOCAL_SUNAT", "0");
                command.Parameters.AddWithValue(":TIP_LOCAL", local.LOC_TIPO ?? string.Empty);
                command.Parameters.AddWithValue(":FEC_APERTURA", (object?)local.LOC_FINICIO ?? DBNull.Value);
                command.Parameters.AddWithValue(":USU_CREACION", "AppSync");
                command.Parameters.AddWithValue(":FEC_CREACION", DateTime.Now);

                command.ExecuteNonQuery();
                return "Inserción exitosa.";
            }
            catch (Exception ex)
            {
                return "Error al crear local::" + ex.Message;
            }
        }

        public string ActualizarLocal(IRS_LOCALES local)
        {
            const string query = @"UPDATE ""SGP"".""MAE_LOCAL""
                                   SET ""NOM_LOCAL""=@NomLocal,
                                       ""FEC_APERTURA""=@FecApertura,
                                       ""USU_MODIFICA""=@UsuModifica,
                                       ""FEC_MODIFICA""=@FecModifica,
                                       ""CEN_COSTO""=@CentroCosto,
                                       ""COD_SAP""=@CodSap
                                   WHERE ""COD_LOCAL""=@CodLocal
                                     AND ""COD_EMPRESA"" NOT IN ('08','09','10','11');";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();

                command.Parameters.AddWithValue("@NomLocal", local.LOC_DESCRIPCION ?? string.Empty);
                command.Parameters.AddWithValue("@FecApertura", (object?)local.LOC_FINICIO ?? DBNull.Value);
                command.Parameters.AddWithValue("@UsuModifica", "AppSync");
                command.Parameters.AddWithValue("@FecModifica", DateTime.Now);
                command.Parameters.AddWithValue("@CentroCosto", local.LOC_CENTROCOSTO ?? string.Empty);
                command.Parameters.AddWithValue("@CodSap", local.LOC_NUMEROSAP ?? string.Empty);
                command.Parameters.AddWithValue("@CodLocal", local.LOC_NUMERO ?? string.Empty);

                command.ExecuteNonQuery();
                return "Actualización exitosa.";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar local::" + ex.Message, ex);
            }
        }

        public List<LocalBctDTO> ObtenerLocalesParaBct()
        {
            var listaLocales = new List<LocalBctDTO>();

            const string query = @"SELECT  CAST(ml.""COD_LOCAL"" AS integer) as ""codigo"",
                                           ml.""NOM_LOCAL"" as ""descripcion"",
                                           ml.""IP""        as ""ip"",
                                           me.""RUC""       as ""ruc_emisor""
                                   FROM ""SGP"".""MAE_LOCAL"" ml
                                   INNER JOIN ""SGP"".""MAE_EMPRESA"" me
                                           ON ml.""COD_EMPRESA"" = me.""COD_EMPRESA""
                                   WHERE me.""RUC"" IN ('20608280333','20601233488')";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();

                var idxCodigo = reader.GetOrdinal("codigo");

                while (reader.Read())
                {                   
                    var dto = new LocalBctDTO
                    {
                        Codigo = reader.IsDBNull(idxCodigo) ? 0 : reader.GetInt32(idxCodigo),
                        Descripcion = reader["descripcion"]?.ToString(),
                        Ip = reader["ip"]?.ToString(),
                        RucEmisor = reader["ruc_emisor"]?.ToString()
                    };

                    listaLocales.Add(dto);
                }

                return listaLocales;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener lista locales::" + ex.Message, ex);
            }
        }
        #endregion

        public string AcualizarParametroFechaNegocio(string fecha, string codEmpresa)
        {
            const string query = @"UPDATE ""SGP"".""PROCESO_PARAM_XEMP""
                                   SET ""VAL_PARAMETRO"" = @Fecha
                                   WHERE ""COD_PARAMETRO"" = '09'
                                     AND ""COD_PROCESO""   IN (29)
                                     AND ""COD_EMPRESA""    = @CodEmpresa";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();

                command.Parameters.AddWithValue("@Fecha", fecha ?? string.Empty);
                command.Parameters.AddWithValue("@CodEmpresa", codEmpresa ?? string.Empty);

                command.ExecuteNonQuery();
                return "Actualizacion fecha exitosa.";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar parametros fecha negocio::" + ex.Message, ex);
            }
        }

        public string AcualizarParametroEstadoConexion(string estado, string codEmpresa)
        {
            const string query = @"UPDATE ""SGP"".""PROCESO_PARAM_XEMP""
                                   SET ""VAL_PARAMETRO"" = @Estado
                                   WHERE ""COD_PARAMETRO"" = '10'
                                     AND ""COD_PROCESO""   IN (29)
                                     AND ""COD_EMPRESA""    = @CodEmpresa";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();

                command.Parameters.AddWithValue("@Estado", estado ?? string.Empty);
                command.Parameters.AddWithValue("@CodEmpresa", codEmpresa ?? string.Empty);

                command.ExecuteNonQuery();
                return "Actualizacion estado conexion exitosa.";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar parametros estado conexion::" + ex.Message, ex);
            }
        }

        public string InsertarTickeGlpi(TicketGlpi ticket)
        {
            const string query = @"INSERT INTO ""SGP"".""TICKET""
                                   (id, fechaproceso, titulo, estado, encuestasatisfaccionfechacreacion, solicitante, tipo, fechaapertura,
                                    ultimaactualizacion, categoria, grupotecnicos, tecnico, localizaciones, fuentessolicitantes, prioridad,
                                    entidad, fechasolucion, elementosasociados, tiempoadueñarse, tiemposolucion, autor, tiemposolucionestadisticas,
                                    tiempoatenderservicio, incidentessociedad1, solicitudsociedad2, incidentesaplicacionprograma, tiempoespera, tipotarea, gruposolicitante)
                                   VALUES
                                   (@Id, @FechaProceso, @Titulo, @Estado, @EncuestaSatisfaccionFechaCreacion, @Solicitante, @Tipo, @FechaApertura,
                                    @UltimaActualizacion, @Categoria, @GrupoTecnicos, @Tecnico, @Localizaciones, @FuentesSolicitantes, @Prioridad,
                                    @Entidad, @FechaSolucion, @ElementosAsociados, @TiempoAdueñarse, @TiempoSolucion, @Autor, @TiempoSolucionEstadisticas,
                                    @TiempoAtenderServicio, @IncidentesSociedad1, @SolicitudSociedad2, @IncidentesAplicacionPrograma, @TiempoEspera, @TipoTarea, @GrupoSolicitante);";

            using var connection = new NpgsqlConnection(_conexionSGP);
            using var command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();

                command.Parameters.AddWithValue("@Id", ticket.Id);
                command.Parameters.AddWithValue("@FechaProceso", DateTime.Today);
                command.Parameters.AddWithValue("@Titulo", ticket.Titulo ?? string.Empty);
                command.Parameters.AddWithValue("@Estado", ticket.Estado ?? string.Empty);
                command.Parameters.AddWithValue("@EncuestaSatisfaccionFechaCreacion", ticket.EncuestaSatisfaccionFechaCreacion);
                command.Parameters.AddWithValue("@Solicitante", ticket.Solicitante ?? string.Empty);
                command.Parameters.AddWithValue("@Tipo", ticket.Tipo ?? string.Empty);
                command.Parameters.AddWithValue("@FechaApertura", ticket.FechaApertura);
                command.Parameters.AddWithValue("@UltimaActualizacion", ticket.UltimaActualizacion);
                command.Parameters.AddWithValue("@Categoria", ticket.Categoria ?? string.Empty);
                command.Parameters.AddWithValue("@GrupoTecnicos", ticket.GrupoTecnicos ?? string.Empty);
                command.Parameters.AddWithValue("@Tecnico", ticket.Tecnico ?? string.Empty);
                command.Parameters.AddWithValue("@Localizaciones", ticket.Localizaciones ?? string.Empty);
                command.Parameters.AddWithValue("@FuentesSolicitantes", ticket.FuentesSolicitantes ?? string.Empty);
                command.Parameters.AddWithValue("@Prioridad", ticket.Prioridad ?? string.Empty);
                command.Parameters.AddWithValue("@Entidad", ticket.Entidad ?? string.Empty);
                command.Parameters.AddWithValue("@FechaSolucion", (object?)ticket.FechaSolucion ?? DBNull.Value);
                command.Parameters.AddWithValue("@ElementosAsociados", ticket.ElementosAsociados ?? string.Empty);
                command.Parameters.AddWithValue("@TiempoAdueñarse", ticket.TiempoAdueñarse);
                command.Parameters.AddWithValue("@TiempoSolucion", ticket.TiempoSolucion);
                command.Parameters.AddWithValue("@Autor", ticket.Autor ?? string.Empty);
                command.Parameters.AddWithValue("@TiempoSolucionEstadisticas", ticket.TiempoSolucionEstadisticas);
                command.Parameters.AddWithValue("@TiempoAtenderServicio", ticket.TiempoAtenderServicio);
                command.Parameters.AddWithValue("@IncidentesSociedad1", ticket.IncidentesSociedad1 ?? string.Empty);
                command.Parameters.AddWithValue("@SolicitudSociedad2", ticket.SolicitudSociedad2 ?? string.Empty);
                command.Parameters.AddWithValue("@IncidentesAplicacionPrograma", ticket.IncidentesAplicacionPrograma ?? string.Empty);
                command.Parameters.AddWithValue("@TiempoEspera", ticket.TiempoEspera);
                command.Parameters.AddWithValue("@TipoTarea", ticket.TipoTarea ?? string.Empty);
                command.Parameters.AddWithValue("@GrupoSolicitante", ticket.GrupoSolicitante ?? string.Empty);

                command.ExecuteNonQuery();
                return "OK";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al InsertarTickeGlpi ::" + ex.Message, ex);
            }
        }

        #region PROCESO_PARAM_XEMP
        public async Task<ProcesoParamPorEmpresa?> ObtenerParametro(string codEmpresa, int codProceso, string codParametro)
        {
            ProcesoParamPorEmpresa? param = null;

            const string query = @"SELECT
                                      ppx.""COD_PROCESO"",
                                      ppx.""COD_EMPRESA"",
                                      ppx.""COD_PARAMETRO"",
                                      ppx.""NOM_PARAMETRO"",
                                      ppx.""VAL_PARAMETRO""
                                   FROM ""SGP"".""PROCESO_PARAM_XEMP"" AS ppx
                                   WHERE ""COD_EMPRESA""  = :COD_EMPRESA
                                     AND ""COD_PROCESO""  = :COD_PROCESO
                                     AND ""COD_PARAMETRO""= :COD_PARAMETRO;";

            await using var connection = new NpgsqlConnection(_conexionSGP);
            await using var command = new NpgsqlCommand(query, connection);

            try
            {
                command.Parameters.AddWithValue(":COD_EMPRESA", codEmpresa ?? (object)DBNull.Value);
                command.Parameters.AddWithValue(":COD_PROCESO", codProceso);
                command.Parameters.AddWithValue(":COD_PARAMETRO", codParametro ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    param = new ProcesoParamPorEmpresa
                    {
                        CodProceso = reader.GetInt32(reader.GetOrdinal("COD_PROCESO")),
                        CodEmpresa = reader["COD_EMPRESA"]?.ToString(),
                        CodParametro = reader["COD_PARAMETRO"]?.ToString(),
                        NomParametro = reader["NOM_PARAMETRO"]?.ToString(),
                        ValParametro = reader["VAL_PARAMETRO"]?.ToString()
                    };
                }

                return param;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener lista parametros::" + ex.Message, ex);
            }
        }

        #endregion

        #region MONITOR_BCT

        public async Task<string> RegistrarMonitorBctCt(MonitorBctDTO monitor)
        {
            const string query = @"INSERT INTO ""SGP"".""MON_BCT_CT""
                                   (""COD_EMPRESA"", ""FECHA_HORA"", ""FECHA"", ""HORA"", ""MINUTO"", ""CANTIDAD"", ""LIMITE"")
                                   VALUES(:COD_EMPRESA, :FECHA_HORA, :FECHA, :HORA, :MINUTO, :CANTIDAD, :LIMITE);";

            await using var connection = new NpgsqlConnection(_conexionSGP);
            await using var command = new NpgsqlCommand(query, connection);

            try
            {
                await connection.OpenAsync();

                command.Parameters.AddWithValue(":COD_EMPRESA", monitor.CodEmpresa ?? string.Empty);
                command.Parameters.AddWithValue(":FECHA_HORA", monitor.FechaHora);
                command.Parameters.AddWithValue(":FECHA", monitor.Fecha);
                command.Parameters.AddWithValue(":HORA", monitor.Hora);
                command.Parameters.AddWithValue(":MINUTO", monitor.Minuto);
                command.Parameters.AddWithValue(":CANTIDAD", Convert.ToInt32(monitor.Cantidad));
                command.Parameters.AddWithValue(":LIMITE", Convert.ToInt32(monitor.Limite));

                await command.ExecuteNonQueryAsync();
                return "Registro exitoso.";
            }
            catch (Exception ex)
            {
                return "Error al registrar monitor bct ct::" + ex.Message;
            }
        }

        #endregion

    }
}
