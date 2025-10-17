using System.Data;
using System.Threading.Tasks;
using System;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
    public interface IRepositorioMovKardex
    {
        Task<DataTable> DescargarMovKardexPorFechas(DateTime fechaInicio, DateTime fechaFin);
    }
}
