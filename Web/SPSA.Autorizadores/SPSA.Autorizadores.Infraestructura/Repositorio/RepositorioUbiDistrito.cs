﻿using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioUbiDistrito : RepositorioGenerico<BCTContexto, UbiDistrito>, IRepositorioUbiDistrito
    {
        public RepositorioUbiDistrito(BCTContexto context) : base(context) { }

        public BCTContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }
    }
}