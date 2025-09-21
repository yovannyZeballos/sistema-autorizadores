using MediatR;
using Serilog;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteAxteroid;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteAxteroid.Dto;
using SPSA.Autorizadores.Infraestructura.Agente.AgenteCen;
using SPSA.Autorizadores.Infraestructura.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Operaciones.Commands
{
    public class DescargarDocumentoElectronicoCommand : IRequest<GenericResponseDTO<byte[]>>
	{
		public string RucEmpresa { get; set; }
		public string NumeroDocumento { get; set; }
		public string TipoDocumento { get; set; }
	}

	public class DescargarDocumentoElectronicoHandler : IRequestHandler<DescargarDocumentoElectronicoCommand, GenericResponseDTO<byte[]>>
	{

		private readonly IAgenteAxteroid _agenteAxteroid;
		private readonly ILogger _logger;

		public DescargarDocumentoElectronicoHandler(IAgenteAxteroid agenteAxteroid)
		{
			_agenteAxteroid = agenteAxteroid;
		}

		public async Task<GenericResponseDTO<byte[]>> Handle(DescargarDocumentoElectronicoCommand request, CancellationToken cancellationToken)
		{
			using (ISGPContexto contexto = new SGPContexto())
			{
				try
				{
					List<ProcesoParametro> parametros = await contexto.RepositorioProcesoParametro
						.Obtener(x => x.CodProceso == Constantes.CodigoProcesoConsultaDocumentoElectronico)
						.AsNoTracking()
						.ToListAsync();
					
					string usuarioServicio = parametros.FirstOrDefault(p => p.CodParametro == Constantes.CodigoParametroUsuarioServicio_DocumentoElectronico)?.ValParametro;
					string claveServicio = parametros.FirstOrDefault(p => p.CodParametro == Constantes.CodigoParametroClaveServicio_DocumentoElectronico)?.ValParametro;

					ConsultaDocumentoElectronicoRespuesta respuesta = await _agenteAxteroid.ConsultarDocumento(new ConsultaDocumentoElectronicoRecurso
					{
						Clave = claveServicio,
						Login = usuarioServicio,
						Folio = request.NumeroDocumento,
						Ruc = request.RucEmpresa,
						TipoDoc = ObtenerTipoDocumento(request.TipoDocumento),
						TipoRetorno = "2"
					});

					if (respuesta.Codigo != "0")
					{
						return new GenericResponseDTO<byte[]>
						{
							Data = null,
							Mensaje = $"Error al descargar el documento: {respuesta.Mensaje}",
							Ok = false
						};
					}

					byte[] archivo = await _agenteAxteroid.DescargarDocumento(respuesta.Mensaje);

					return new GenericResponseDTO<byte[]>
					{
						Data = archivo,
						Mensaje = "Documento descargado exitosamente",
						Ok = true
					};
				}
				catch (Exception ex)
				{
					_logger.Error(ex, ex.Message);
					return new GenericResponseDTO<byte[]>
					{
						Data = null,
						Mensaje = ex.Message,
						Ok = false
					};
				}
				

			}
		}

		private string ObtenerTipoDocumento(string valor)
		{
			switch (valor)
			{
				case "TFC":
					return "01";
				case "BLT":
					return "03";
				case "NCR":
					return "07";
				default:
					return string.Empty;
			}
		}
	}
}
