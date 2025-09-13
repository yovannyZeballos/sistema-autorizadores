using MediatR;
using Microsoft.IdentityModel.Tokens;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Auxiliar;
using SPSA.Autorizadores.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.Operaciones.Queries
{
    public class ListarDocumentosElectronicosQuery : IRequest<GenericResponseDTO<PagedResult<DocumentoElectronico>>>
	{
		public string CodEmpresa { get; set; }
		public string Codlocal { get; set; }
		public DateTime FechaInicio { get; set; }
		public DateTime FechaFin { get; set; }
		public string TipoDocCliente { get; set; }
		public string NroDocCliente { get; set; }
		public string Cajero { get; set; }
		public string Caja { get; set; }
		public string NroTransaccion { get; set; }
		public int NumeroPagina { get; set; }
		public int TamañoPagina { get; set; }
		public string Busqueda { get; set; }
	}

	public class ListarDocumentosElectronicosHandler : IRequestHandler<ListarDocumentosElectronicosQuery, GenericResponseDTO<PagedResult<DocumentoElectronico>>>
	{

		public async Task<GenericResponseDTO<PagedResult<DocumentoElectronico>>> Handle(ListarDocumentosElectronicosQuery request, CancellationToken cancellationToken)
		{
			var respuesta = new GenericResponseDTO<PagedResult<DocumentoElectronico>> { Ok = true };
			try
			{
				var documentos = await ObtenerDatosPruebaPaginado(request.NumeroPagina, request.TamañoPagina);
			    int totalRegistros = await ObtenerTotalRegistrosPrueba();
				respuesta.Data = new PagedResult<DocumentoElectronico>
				{
					Items = documentos.Where(x => x.ToString().ToUpper().Contains((request.Busqueda ?? "").ToUpper())).ToList(),
					PageNumber = request.NumeroPagina,
					PageSize = request.TamañoPagina,
					TotalPages = totalRegistros,
					TotalRecords = totalRegistros
				};
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}
			return respuesta;
		}

		private static async Task<List<DocumentoElectronico>> ObtenerDatosPrueba()
		{
			await Task.Delay(1);
			return new List<DocumentoElectronico>
			{
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 15), Caja = "CAJA01", Importe = 125.50m, DocElectronico = "DE12345678", MedioPago = "EFECTIVO", Cajero = "JPEREZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000001", Local = "001" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 15), Caja = "CAJA02", Importe = 250.75m, DocElectronico = "DE12345679", MedioPago = "TARJETA_CREDITO", Cajero = "MGARCIA", TipoDocumento = "FACTURA", NroDocumento = "F001-000001", Local = "002" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 14), Caja = "CAJA03", Importe = 89.20m, DocElectronico = "DE12345680", MedioPago = "TARJETA_DEBITO", Cajero = "LRODRIGUEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000002", Local = "003" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 14), Caja = "CAJA01", Importe = 456.30m, DocElectronico = "DE12345681", MedioPago = "YAPE", Cajero = "CLOPEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000002", Local = "001" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 13), Caja = "CAJA04", Importe = 78.90m, DocElectronico = "DE12345682", MedioPago = "PLIN", Cajero = "AMARTINEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000003", Local = "004" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 13), Caja = "CAJA02", Importe = 199.99m, DocElectronico = "DE12345683", MedioPago = "EFECTIVO", Cajero = "RGONZALEZ", TipoDocumento = "NOTA_CREDITO", NroDocumento = "N001-000001", Local = "002" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 12), Caja = "CAJA05", Importe = 324.15m, DocElectronico = "DE12345684", MedioPago = "TRANSFERENCIA", Cajero = "SFERNANDEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000004", Local = "005" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 12), Caja = "CAJA03", Importe = 567.80m, DocElectronico = "DE12345685", MedioPago = "TARJETA_CREDITO", Cajero = "JPEREZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000003", Local = "003" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 11), Caja = "CAJA01", Importe = 145.25m, DocElectronico = "DE12345686", MedioPago = "TARJETA_DEBITO", Cajero = "MGARCIA", TipoDocumento = "BOLETA", NroDocumento = "B001-000005", Local = "001" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 11), Caja = "CAJA06", Importe = 890.50m, DocElectronico = "DE12345687", MedioPago = "EFECTIVO", Cajero = "LRODRIGUEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000004", Local = "006" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 10), Caja = "CAJA04", Importe = 67.45m, DocElectronico = "DE12345688", MedioPago = "YAPE", Cajero = "CLOPEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000006", Local = "004" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 10), Caja = "CAJA07", Importe = 234.60m, DocElectronico = "DE12345689", MedioPago = "PLIN", Cajero = "AMARTINEZ", TipoDocumento = "NOTA_DEBITO", NroDocumento = "N001-000002", Local = "007" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 09), Caja = "CAJA02", Importe = 156.30m, DocElectronico = "DE12345690", MedioPago = "TRANSFERENCIA", Cajero = "RGONZALEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000007", Local = "002" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 09), Caja = "CAJA08", Importe = 445.75m, DocElectronico = "DE12345691", MedioPago = "TARJETA_CREDITO", Cajero = "SFERNANDEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000005", Local = "008" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 08), Caja = "CAJA05", Importe = 98.40m, DocElectronico = "DE12345692", MedioPago = "TARJETA_DEBITO", Cajero = "JPEREZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000008", Local = "005" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 08), Caja = "CAJA03", Importe = 678.90m, DocElectronico = "DE12345693", MedioPago = "EFECTIVO", Cajero = "MGARCIA", TipoDocumento = "FACTURA", NroDocumento = "F001-000006", Local = "003" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 07), Caja = "CAJA09", Importe = 123.15m, DocElectronico = "DE12345694", MedioPago = "YAPE", Cajero = "LRODRIGUEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000009", Local = "009" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 07), Caja = "CAJA01", Importe = 789.25m, DocElectronico = "DE12345695", MedioPago = "PLIN", Cajero = "CLOPEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000007", Local = "001" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 06), Caja = "CAJA10", Importe = 87.65m, DocElectronico = "DE12345696", MedioPago = "TRANSFERENCIA", Cajero = "AMARTINEZ", TipoDocumento = "NOTA_CREDITO", NroDocumento = "N001-000003", Local = "010" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 06), Caja = "CAJA06", Importe = 345.80m, DocElectronico = "DE12345697", MedioPago = "TARJETA_CREDITO", Cajero = "RGONZALEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000010", Local = "006" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 05), Caja = "CAJA04", Importe = 267.45m, DocElectronico = "DE12345698", MedioPago = "TARJETA_DEBITO", Cajero = "SFERNANDEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000008", Local = "004" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 05), Caja = "CAJA11", Importe = 156.70m, DocElectronico = "DE12345699", MedioPago = "EFECTIVO", Cajero = "JPEREZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000011", Local = "011" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 04), Caja = "CAJA07", Importe = 534.20m, DocElectronico = "DE12345700", MedioPago = "YAPE", Cajero = "MGARCIA", TipoDocumento = "FACTURA", NroDocumento = "F001-000009", Local = "007" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 04), Caja = "CAJA02", Importe = 98.55m, DocElectronico = "DE12345701", MedioPago = "PLIN", Cajero = "LRODRIGUEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000012", Local = "002" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 03), Caja = "CAJA12", Importe = 423.90m, DocElectronico = "DE12345702", MedioPago = "TRANSFERENCIA", Cajero = "CLOPEZ", TipoDocumento = "NOTA_DEBITO", NroDocumento = "N001-000004", Local = "012" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 03), Caja = "CAJA08", Importe = 189.30m, DocElectronico = "DE12345703", MedioPago = "TARJETA_CREDITO", Cajero = "AMARTINEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000013", Local = "008" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 02), Caja = "CAJA05", Importe = 567.15m, DocElectronico = "DE12345704", MedioPago = "TARJETA_DEBITO", Cajero = "RGONZALEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000010", Local = "005" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 02), Caja = "CAJA13", Importe = 134.75m, DocElectronico = "DE12345705", MedioPago = "EFECTIVO", Cajero = "SFERNANDEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000014", Local = "013" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 01), Caja = "CAJA03", Importe = 298.60m, DocElectronico = "DE12345706", MedioPago = "YAPE", Cajero = "JPEREZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000011", Local = "003" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 12, 01), Caja = "CAJA09", Importe = 76.85m, DocElectronico = "DE12345707", MedioPago = "PLIN", Cajero = "MGARCIA", TipoDocumento = "NOTA_CREDITO", NroDocumento = "N001-000005", Local = "009" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 30), Caja = "CAJA14", Importe = 445.20m, DocElectronico = "DE12345708", MedioPago = "TRANSFERENCIA", Cajero = "LRODRIGUEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000015", Local = "014" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 30), Caja = "CAJA01", Importe = 167.40m, DocElectronico = "DE12345709", MedioPago = "TARJETA_CREDITO", Cajero = "CLOPEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000012", Local = "001" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 29), Caja = "CAJA10", Importe = 323.55m, DocElectronico = "DE12345710", MedioPago = "TARJETA_DEBITO", Cajero = "AMARTINEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000016", Local = "010" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 29), Caja = "CAJA06", Importe = 89.95m, DocElectronico = "DE12345711", MedioPago = "EFECTIVO", Cajero = "RGONZALEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000013", Local = "006" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 28), Caja = "CAJA15", Importe = 234.70m, DocElectronico = "DE12345712", MedioPago = "YAPE", Cajero = "SFERNANDEZ", TipoDocumento = "NOTA_DEBITO", NroDocumento = "N001-000006", Local = "015" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 28), Caja = "CAJA04", Importe = 456.30m, DocElectronico = "DE12345713", MedioPago = "PLIN", Cajero = "JPEREZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000017", Local = "004" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 27), Caja = "CAJA11", Importe = 178.25m, DocElectronico = "DE12345714", MedioPago = "TRANSFERENCIA", Cajero = "MGARCIA", TipoDocumento = "FACTURA", NroDocumento = "F001-000014", Local = "011" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 27), Caja = "CAJA07", Importe = 67.80m, DocElectronico = "DE12345715", MedioPago = "TARJETA_CREDITO", Cajero = "LRODRIGUEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000018", Local = "007" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 26), Caja = "CAJA16", Importe = 589.40m, DocElectronico = "DE12345716", MedioPago = "TARJETA_DEBITO", Cajero = "CLOPEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000015", Local = "016" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 26), Caja = "CAJA02", Importe = 145.60m, DocElectronico = "DE12345717", MedioPago = "EFECTIVO", Cajero = "AMARTINEZ", TipoDocumento = "NOTA_CREDITO", NroDocumento = "N001-000007", Local = "002" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 25), Caja = "CAJA12", Importe = 267.85m, DocElectronico = "DE12345718", MedioPago = "YAPE", Cajero = "RGONZALEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000019", Local = "012" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 25), Caja = "CAJA08", Importe = 398.15m, DocElectronico = "DE12345719", MedioPago = "PLIN", Cajero = "SFERNANDEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000016", Local = "008" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 24), Caja = "CAJA17", Importe = 123.90m, DocElectronico = "DE12345720", MedioPago = "TRANSFERENCIA", Cajero = "JPEREZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000020", Local = "017" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 24), Caja = "CAJA05", Importe = 456.70m, DocElectronico = "DE12345721", MedioPago = "TARJETA_CREDITO", Cajero = "MGARCIA", TipoDocumento = "FACTURA", NroDocumento = "F001-000017", Local = "005" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 23), Caja = "CAJA13", Importe = 89.25m, DocElectronico = "DE12345722", MedioPago = "TARJETA_DEBITO", Cajero = "LRODRIGUEZ", TipoDocumento = "NOTA_DEBITO", NroDocumento = "N001-000008", Local = "013" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 23), Caja = "CAJA03", Importe = 234.50m, DocElectronico = "DE12345723", MedioPago = "EFECTIVO", Cajero = "CLOPEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000021", Local = "003" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 22), Caja = "CAJA18", Importe = 567.35m, DocElectronico = "DE12345724", MedioPago = "YAPE", Cajero = "AMARTINEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000018", Local = "018" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 22), Caja = "CAJA09", Importe = 178.80m, DocElectronico = "DE12345725", MedioPago = "PLIN", Cajero = "RGONZALEZ", TipoDocumento = "BOLETA", NroDocumento = "B001-000022", Local = "009" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 21), Caja = "CAJA14", Importe = 345.15m, DocElectronico = "DE12345726", MedioPago = "TRANSFERENCIA", Cajero = "SFERNANDEZ", TipoDocumento = "FACTURA", NroDocumento = "F001-000019", Local = "014" },
				new DocumentoElectronico { Fecha = new DateTime(2024, 11, 21), Caja = "CAJA01", Importe = 98.75m, DocElectronico = "DE12345727", MedioPago = "TARJETA_CREDITO", Cajero = "JPEREZ", TipoDocumento = "NOTA_CREDITO", NroDocumento = "N001-000009", Local = "001" }
			};
		}

		private static async Task<List<DocumentoElectronico>> ObtenerDatosPruebaPaginado(int numeroPagina, int tamañoPagina)
		{
			var todosLosDocumentos = await ObtenerDatosPrueba();

			// Calcular la paginación
			var skip = (numeroPagina - 1) * tamañoPagina;
			return todosLosDocumentos.Skip(skip).Take(tamañoPagina).ToList();
		}

		private static async Task<int> ObtenerTotalRegistrosPrueba()
		{
			var todosLosDocumentos = await ObtenerDatosPrueba();
			return todosLosDocumentos.Count;
		}

	}
}
