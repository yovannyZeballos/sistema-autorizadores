﻿using SPSA.Autorizadores.Dominio.Entidades;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Dominio.Contrato.Repositorio
{
	public interface IRepositorioTransactionXmlCT2
	{
		Task<TransactionXmlCT2> Obtener(string cadenaConexion);
	}
}