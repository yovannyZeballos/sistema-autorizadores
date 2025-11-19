using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs.Servidor
{
    public class ServidorVirtualDto
    {
        public long Id { get; set; }
        public long HostSerieId { get; set; }

        public string Hostname { get; set; }
        public string Ip { get; set; }
        public decimal? RamGb { get; set; }
        public int? VCores { get; set; }
        public string HddTotal { get; set; }

        public long? SoId { get; set; }
        public string NomSo { get; set; }

        public short? PlataformaId { get; set; }
        public string NomPlataforma { get; set; }

        public string IndActivo { get; set; } // 'S' / 'N'
        public string Url { get; set; }

        public string UsuCreacion { get; set; }
        public DateTime FecCreacion { get; set; }
        public string UsuModifica { get; set; }
        public DateTime? FecModifica { get; set; }
    }
}
