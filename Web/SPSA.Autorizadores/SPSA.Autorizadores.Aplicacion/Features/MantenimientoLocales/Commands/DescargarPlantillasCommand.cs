using MediatR;
using SPSA.Autorizadores.Aplicacion.DTO;
using System;
using System.IO.Compression;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.MantenimientoLocales.Commands
{
    public class DescargarPlantillasCommand : IRequest<DescargarPlantillasDTO>
    {
        public string Carpeta { get; set; }
    }

    public class DescargarPlantillasHandler : IRequestHandler<DescargarPlantillasCommand, DescargarPlantillasDTO>
    {
        public async Task<DescargarPlantillasDTO> Handle(DescargarPlantillasCommand request, CancellationToken cancellationToken)
        {
            var task = Task.Run(() =>
            {
                var respuesta = new DescargarPlantillasDTO();
                try
                {
                    var carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, request.Carpeta);

                    using (MemoryStream zipToOpen = new MemoryStream())
                    {
                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                        {
                            ZipArchiveEntry readmeEntry;
                            DirectoryInfo d = new DirectoryInfo(carpeta);
                            FileInfo[] Files = d.GetFiles("*");
                            foreach (FileInfo file in Files)
                            {
                                readmeEntry = archive.CreateEntryFromFile(file.FullName, file.Name);
                            }
                        }

                        respuesta.Archivo = Convert.ToBase64String(zipToOpen.ToArray());
                        respuesta.Ok = true;
                        respuesta.NombreArchivo = $"{request.Carpeta}.zip";
                    }
                }
                catch (Exception ex)
                {
                    respuesta.Ok = false;
                    respuesta.Mensaje = ex.Message;
                }

                return respuesta;
            });


            return await task;


        }
    }
}
