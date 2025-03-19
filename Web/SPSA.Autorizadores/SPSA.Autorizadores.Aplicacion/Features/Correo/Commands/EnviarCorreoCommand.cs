using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Logger;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Contexto;

namespace SPSA.Autorizadores.Aplicacion.Features.Correo.Commands
{
    public class EnviarCorreoCommand : IRequest<RespuestaComunDTO>
    {
        public int CodProceso { get; set; }
        public string CodEmpresa { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class EnviarCorreoHandler : IRequestHandler<EnviarCorreoCommand, RespuestaComunDTO>
    {
        private readonly ISGPContexto _contexto;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EnviarCorreoHandler(IMapper mapper)
        {
            _mapper = mapper;
            _contexto = new SGPContexto();
            _logger = SerilogClass._log;
        }

        public class EmailSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool EnableSsl { get; set; }
            public string To { get; set; }
        }

        private EmailSettings GetEmailSettings(int codProceso, string codEmpresa)
        {
            var parametros = _contexto.RepositorioProcesoParametroEmpresa.Obtener(x => x.CodProceso == codProceso &&
                                                        x.CodEmpresa == codEmpresa).ToList();

            var host = parametros.FirstOrDefault(x => x.CodParametro == "01")?.ValParametro ?? "";
            var portString = parametros.FirstOrDefault(x => x.CodParametro == "02")?.ValParametro ?? "25";
            if (!int.TryParse(portString, out int port))
            {
                port = 25;
            }
            var username = parametros.FirstOrDefault(x => x.CodParametro == "03")?.ValParametro ?? "";
            var password = parametros.FirstOrDefault(x => x.CodParametro == "04")?.ValParametro ?? "";
            var sslString = parametros.FirstOrDefault(x => x.CodParametro == "05")?.ValParametro ?? "0";
            bool enableSsl = sslString == "true";
            var to = parametros.FirstOrDefault(x => x.CodParametro == "06")?.ValParametro ?? "";


            return new EmailSettings
            {
                Host = host,
                Port = port,
                Username = username,
                Password = password,
                EnableSsl = enableSsl,
                To = to
            };
        }

        public async Task<RespuestaComunDTO> Handle(EnviarCorreoCommand request, CancellationToken cancellationToken)
        {
            var respuesta = new RespuestaComunDTO { Ok = true };
            try
            {
                var emailSettings = GetEmailSettings(request.CodProceso, request.CodEmpresa);

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = emailSettings.Host;
                    smtp.Port = emailSettings.Port;
                    smtp.EnableSsl = emailSettings.EnableSsl;
                    smtp.Credentials = new System.Net.NetworkCredential(emailSettings.Username, emailSettings.Password);

                    var mail = new MailMessage
                    {
                        From = new MailAddress(emailSettings.Username, "Soporte BackOffice"),
                        Subject = request.Subject,
                        Body = request.Message,
                        IsBodyHtml = false // Cambia a true si deseas enviar contenido HTML
                    };

                    mail.To.Add(emailSettings.To);

                    await smtp.SendMailAsync(mail);
                }

                respuesta.Mensaje = "Correo enviado correctamente.";
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "Error al enviar el correo: " + ex.Message;
                _logger.Error(ex, "Error al enviar el correo.");
            }

            return respuesta;
        }
    }
}
