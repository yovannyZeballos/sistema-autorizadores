using ExcelDataReader;
using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Dominio.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Web;
using SPSA.Autorizadores.Aplicacion.Extensiones;
using System.Globalization;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands
{
	public class ImportarInventarioServidorCommand : IRequest<RespuestaComunExcelDTO>
	{
		public HttpPostedFileBase Archivo { get; set; }
		public string Usuario { get; set; }
	}

	public class ImportarInventarioServidorHandler : IRequestHandler<ImportarInventarioServidorCommand, RespuestaComunExcelDTO>
	{
		private readonly IRepositorioInventarioServidor _repositorioInventarioServidor;
		private readonly IRepositorioInventarioServidorVirtual _repositorioInventarioServidorVirtual;

		public ImportarInventarioServidorHandler(IRepositorioInventarioServidor repositorioInventarioServidor, IRepositorioInventarioServidorVirtual repositorioInventarioServidorVirtual)
		{
			_repositorioInventarioServidor = repositorioInventarioServidor;
			_repositorioInventarioServidorVirtual = repositorioInventarioServidorVirtual;
		}

		public async Task<RespuestaComunExcelDTO> Handle(ImportarInventarioServidorCommand request, CancellationToken cancellationToken)
		{
			var respuesta = new RespuestaComunExcelDTO { Errores = new List<ErroresExcelDTO>() };
			try
			{
				using (var reader = ExcelReaderFactory.CreateReader(request.Archivo.InputStream))
				{
					var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
					{
						FilterSheet = (tableReader, sheetIndex) =>
						{
							var name = tableReader.Name.ToLower();
							if (name.Contains("plantilla"))
								return true;
							else
								return false;
						},
						ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
						{
							UseHeaderRow = true,
						},
						UseColumnDataType = true
					}).ToAllStringFields();


					if (ds.Tables.Count > 0)
					{
						var codEmpresaAux = "";
						var codFormatoAux = "";
						var codLocalAux = "";
						var numServerAux = "";

						var fila = 1;
						foreach (DataRow row in ds.Tables[0].Rows)
						{
							var codEmpresa = row["COD_EMPRESA"].ToString();
							var codFormato = row["COD_FORMATO"].ToString();
							var codLocal = row["COD_LOCAL"].ToString();
							var numServer = row["NUM_SERVER"].ToString();

							try
							{
								if (codEmpresa != codEmpresaAux|| codFormato != codFormatoAux || codLocal != codLocalAux || numServer != numServerAux)
								{
									//Insertar servidor
									await _repositorioInventarioServidor.Insertar(new Dominio.Entidades.InventarioServidor(codEmpresa, codFormato, codLocal, numServer,
										row["TIP_SERVER"].ToString(), row["COD_MARCA"].ToString(), row["COD_MODELO"].ToString(), row["HOSTNAME"].ToString(), row["SERIE"].ToString(),
										row["IP"].ToString(), Convert.ToDecimal(row["RAM"]), Convert.ToDecimal(row["HDD"]), row["COD_SO"].ToString(), row["REPLICA"].ToString(),
										row["IP_REMOTA"].ToString(), Convert.ToDecimal(row["ANTIGUEDAD"]), row["OBSERVACIONES"].ToString(), row["ANTIVIRUS"].ToString(), request.Usuario));
								}

								//Insertar virtual
								await _repositorioInventarioServidorVirtual.Insertar(new InventarioServidorVirtual(0, codEmpresa, codFormato, codLocal, numServer,
									row["VIRTUAL_TIPO"].ToString(), Convert.ToDecimal(row["VIRTUAL_RAM"]), Convert.ToDecimal(row["VIRTUAL_CPU"]), 
									Convert.ToDecimal(row["VIRTUAL_HDD"]), row["VIRTUAL_SO"].ToString(), request.Usuario));
								
							}
							catch (Exception ex)
							{
								respuesta.Errores.Add(new ErroresExcelDTO
								{
									Fila = fila,
									Mensaje = ex.Message
								});
							}

							codEmpresaAux = codEmpresa;
							codFormatoAux = codFormato;
							codLocalAux = codLocal;
							numServerAux = numServer;
							fila++;
						}
					}

					if (respuesta.Errores.Count == 0)
					{
						respuesta.Ok = true;
						respuesta.Mensaje = "Archivo importado correctamente";
					}
					else
					{
						respuesta.Ok = false;
						respuesta.Mensaje = "Se encontraron algunos errores en el archivo";
					}

				}
			}
			catch (Exception ex)
			{
				respuesta.Ok = false;
				respuesta.Mensaje = ex.Message;
			}

			return respuesta;
		}
	}
}
