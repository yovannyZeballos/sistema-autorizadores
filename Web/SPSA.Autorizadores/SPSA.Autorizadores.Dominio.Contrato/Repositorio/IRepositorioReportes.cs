﻿using System;
using System.Data;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioReportes
    {
        Task<DataTable> ListarLocalesCambioPrecio(string codLocal, DateTime fechaInicio, DateTime fechaFin);
        Task<DataTable> ListarLocalesNotaCredito(string codLocal, DateTime fechaInicio, DateTime fechaFin);
        Task<DataTable> ListarValesRedimidosAsync(string codLocal, DateTime fechaInicio, DateTime fechaFin);
        Task<DataTable> ListarValesRedimidosPaginadoAsync(string codLocal, DateTime fechaInicio, DateTime fechaFin, int startRow, int endRow);
        Task<DataTable> ListarAutorizadoresAsync();
        Task<DataTable> ListarAutorizadoresPaginadoAsync(int startRow, int endRow);
        Task<DataTable> ListarCajerosAsync();
        Task<DataTable> ListarCajerosPaginadoAsync(int startRow, int endRow);
    }
}
