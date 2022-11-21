using AutoMapper;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Seguridad.Commands
{
    public class LoginCommand : IRequest<UsuarioDTO>
    {
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginHandler : IRequestHandler<LoginCommand, UsuarioDTO>
    {
        private readonly IRepositorioSeguridad _repositorioSeguridad;
        private readonly IMapper _mapper;

        public LoginHandler(IRepositorioSeguridad repositorioSeguridad, IMapper mapper)
        {
            _repositorioSeguridad = repositorioSeguridad;
            _mapper = mapper;
        }

        public Task<UsuarioDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var codigoSistema = Convert.ToInt32(ConfigurationManager.AppSettings["SistemaCodigo"]);
            var abreviatura = ConfigurationManager.AppSettings["SistemaAbreviatura"].ToString();
            string instancia = ConfigurationManager.AppSettings["instancia"];
            string user_bd = ConfigurationManager.AppSettings["user_bd"];
            string passw_bd = ConfigurationManager.AppSettings["passw_bd"];

            byte[] bInst = pe.oechsle.sca.Seguridad.hexToByte(instancia, "-");
            byte[] bUsBd = pe.oechsle.sca.Seguridad.hexToByte(user_bd, "-");
            byte[] bPwBD = pe.oechsle.sca.Seguridad.hexToByte(passw_bd, "-");

            pe.oechsle.sca.Seguridad objSegur = new pe.oechsle.sca.Seguridad(bInst, bUsBd, bPwBD);
            pe.oechsle.Entity.Usuario objUser = objSegur.login(abreviatura, request.Usuario, request.Password);

            var usuarioDTO = _mapper.Map<UsuarioDTO>(objUser);

            usuarioDTO.Locales.Clear();

            if (objUser.TablasTipo.Count == 0)
            {
                usuarioDTO.Mensaje = "Usuario no asignado a ningun Tipo de Tabla.";
                usuarioDTO.Ok = false;
            }


            foreach (var tabla in objUser.TablasTipo)
            {
                if(tabla.abreviatura == "SUCS")
                {
                    if (tabla.Detalles.Count == 0)
                    {
                        usuarioDTO.Mensaje = "Usuario no asignado a ninguna Tienda.";
                        usuarioDTO.Ok = false;
                        break;
                    }


                    foreach (var item in tabla.Detalles)
                    {
                        usuarioDTO.Locales.Add(new LocalDTO
                        {
                            Codigo = item.codigo,
                            Nombre = item.detalle
                        });
                    }
                }
            }

            return Task.FromResult(usuarioDTO);
        }
    }
}
