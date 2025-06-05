using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioMdrBinesIzipay : RepositorioGenerico<SGPContexto, Mdr_BinesIzipay>, IRepositorioMdrBinesIzipay
    {
        public RepositorioMdrBinesIzipay(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }

        public async Task<List<Mdr_BinesIzipay>> ObtenerConsolidadoBinesAsync(string codEmpresa, string numAno)
        {
            var lista = new List<Mdr_BinesIzipay>();

            using (var connection = new NpgsqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();

                string sql = @"SELECT * FROM ""SGP"".sf_mdr_consolidado_bines(@p_cod_empresa, @p_num_ano);";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@p_cod_empresa", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = codEmpresa });
                    command.Parameters.Add(new NpgsqlParameter("@p_num_ano", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = numAno });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                lista.Add(new Mdr_BinesIzipay
                                {
                                    CodEmpresa = reader["COD_EMPRESA"] != DBNull.Value ? reader["COD_EMPRESA"].ToString() : string.Empty,
                                    NumAno = reader["NUM_ANO"] != DBNull.Value ? reader["NUM_ANO"].ToString() : string.Empty,
                                    NumBin6 = reader["NUM_BIN_6"] != DBNull.Value ? reader["NUM_BIN_6"].ToString() : string.Empty,
                                    NumBin8 = reader["NUM_BIN_8"] != DBNull.Value ? reader["NUM_BIN_8"].ToString() : string.Empty,
                                    NomTarjeta = reader["CLASIFICACION_NOMBRE"] != DBNull.Value ? reader["CLASIFICACION_NOMBRE"].ToString() : string.Empty,
                                    BancoEmisor = reader["BANCO_EMISOR"] != DBNull.Value ? reader["BANCO_EMISOR"].ToString() : string.Empty,
                                    Tipo = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : string.Empty,
                                    FactorMdr = reader["FACTOR"] != DBNull.Value ? Convert.ToDecimal(reader["FACTOR"]) : 0m,
                                    CodOperador = reader["COD_OPERADOR"] != DBNull.Value ? reader["COD_OPERADOR"].ToString() : string.Empty
                                });
                            }
                        }
                    }
                }
            }

            return lista;
        }
    }
}
