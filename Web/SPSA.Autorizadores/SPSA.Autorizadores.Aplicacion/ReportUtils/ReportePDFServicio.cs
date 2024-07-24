using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Zen.Barcode;

namespace SPSA.Autorizadores.Aplicacion.ReportUtils
{
	public class ReportePDFServicio
	{
		public static string GenerarReportePDFBase64<T>(List<T> reportData, string dataSetName, Dictionary<string, string> parametros, string reportPath)
		{
			if (reportData == null || reportData.Count == 0)
			{
				throw new ArgumentException("Los datos del reporte no pueden estar vacíos.");
			}

			LocalReport report = new LocalReport();
			report.ReportPath = reportPath;

			// Limpiar los data sources previos si los hay
			report.DataSources.Clear();

			var parametrosReporte = parametros.Select(p => new ReportParameter(p.Key, p.Value)).ToList();

			report.SetParameters(parametrosReporte);

			// Crear un nuevo ReportDataSource y asignarlo al reporte
			ReportDataSource source = new ReportDataSource(dataSetName, reportData);
			report.DataSources.Add(source);

			// Renderizar el reporte a PDF
			string mimeType, encoding, fileNameExtension;
			string[] streams;
			Warning[] warnings;
			byte[] renderedBytes;

			// Especificar el formato del reporte, en este caso PDF
			renderedBytes = report.Render(
				"PDF", null, out mimeType, out encoding, out fileNameExtension,
				out streams, out warnings);

			return Convert.ToBase64String(renderedBytes);
		}

		public static string GenerarCodigoDeBarras(string datos)
		{
			BarcodeDraw barcode = BarcodeDrawFactory.Code39WithoutChecksum;
			var imagenCodigoBarras = barcode.Draw(datos, 30);

			using (var ms = new MemoryStream())
			{
				imagenCodigoBarras.Save(ms, ImageFormat.Png);
				return Convert.ToBase64String(ms.ToArray());
			}
		}
	}
}
