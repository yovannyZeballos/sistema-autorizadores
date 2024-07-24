using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
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
        private readonly ISGPContexto _bCTContexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public LoginHandler(IMapper mapper)
        {
            _mapper = mapper;
            _bCTContexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public async Task<UsuarioDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
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
            usuarioDTO.MenusAsociados = await ObtenerMenusAsociados(abreviatura, request.Usuario);
            usuarioDTO.Locales.Clear();

            if (objUser.TablasTipo.Count == 0)
            {
                usuarioDTO.Mensaje = "Usuario no asignado a ningun Tipo de Tabla.";
                usuarioDTO.Ok = false;
            }


            foreach (var tabla in objUser.TablasTipo)
            {
                if (tabla.abreviatura == "SUCS")
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

            if (usuarioDTO.Ok)
            {
                await RegistrarFechaHoraIngreso(request.Usuario);
            }

            return usuarioDTO;
        }

        private async Task<List<ListarMenuDTO>> ObtenerMenusAsociados(string siglaSistema, string codUsuario)
        {
            var response = new List<ListarMenuDTO>();
            try
            {
                var sistema = await _bCTContexto.RepositorioSegSistema
                    .Obtener(x => x.Sigla == siglaSistema)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                var perfiles = await _bCTContexto.RepositorioSegPerfilUsuario
                    .Obtener(x => x.CodUsuario == codUsuario)
                    .AsNoTracking()
                    .Select(x => x.CodPerfil).ToListAsync();

                var menus = await _bCTContexto.RepositorioSegPerfilMenu
                    .Obtener(x => x.CodSistema == sistema.CodSistema && perfiles.Contains(x.CodPerfil))
                    .AsNoTracking()
                    .Include(x => x.Menu)
                    .Select(x => x.Menu).Distinct().ToListAsync();

                response = _mapper.Map<List<ListarMenuDTO>>(menus);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception("Error al obtener los menus asociados al usuario", ex);
            }
            return response;
        }


        private async Task RegistrarFechaHoraIngreso(string codUsuario)
        {
            var usuario = await _bCTContexto.RepositorioSegUsuario.Obtener(x => x.CodUsuario == codUsuario).AsNoTracking().FirstOrDefaultAsync();
            usuario.FecIngreso = DateTime.Now;
            usuario.HoraIngreso = DateTime.Now.TimeOfDay;
            _bCTContexto.RepositorioSegUsuario.Actualizar(usuario);
            await _bCTContexto.GuardarCambiosAsync();
        }
    }
}
