using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
    public class SrvPlataformaVmTypeConfiguration : EntityTypeConfiguration<SrvPlataformaVm>
    {
        public SrvPlataformaVmTypeConfiguration()
        {
            ToTable("srv_plataforma_vm", "SGP");

            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("id");
            Property(x => x.NomPlataforma).HasColumnName("nom_plataforma");
        }
    }
}
