﻿using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Infraestructura.Repositorio
{
    public class RepositorioSolicitudUsuarioASR : RepositorioGenerico<SGPContexto, ASR_SolicitudUsuario>, IRepositorioSolicitudUsuarioASR
    {
        public RepositorioSolicitudUsuarioASR(SGPContexto context) : base(context) { }

        public SGPContexto AppDBMyBDContext
        {
            get { return _contexto; }
        }
    }
}
