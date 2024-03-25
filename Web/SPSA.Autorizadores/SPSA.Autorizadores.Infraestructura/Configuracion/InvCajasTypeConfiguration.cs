using SPSA.Autorizadores.Dominio.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace SPSA.Autorizadores.Infraestructura.Configuracion
{
	public class InvCajasTypeConfiguration : EntityTypeConfiguration<InvCajas>
	{
		public InvCajasTypeConfiguration()
		{
			ToTable("INV_CAJAS", "SGP");

			HasKey(x => new { x.CodEmpresa, x.CodCadena, x.CodRegion, x.CodZona, x.CodLocal, x.NumCaja, x.CodActivo });

			Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA").IsRequired().HasMaxLength(2);
			Property(x => x.CodCadena).HasColumnName("COD_CADENA").IsRequired().HasMaxLength(2);
			Property(x => x.CodRegion).HasColumnName("COD_REGION").IsRequired().HasMaxLength(2);
			Property(x => x.CodZona).HasColumnName("COD_ZONA").IsRequired().HasMaxLength(3);
			Property(x => x.CodLocal).HasColumnName("COD_LOCAL").IsRequired().HasMaxLength(4);
			Property(x => x.NumCaja).HasColumnName("NUM_CAJA").IsRequired();
			Property(x => x.CodActivo).HasColumnName("COD_ACTIVO").IsRequired().HasMaxLength(3);
			Property(x => x.CodModelo).HasColumnName("COD_MODELO").HasMaxLength(50);
			Property(x => x.CodSerie).HasColumnName("COD_SERIE").HasMaxLength(50);
			Property(x => x.NumAdenda).HasColumnName("NUM_ADENDA").HasMaxLength(10);
			Property(x => x.FecGarantia).HasColumnName("FEC_GARANTIA");
			Property(x => x.TipEstado).HasColumnName("TIP_ESTADO").HasMaxLength(1);
			Property(x => x.TipProcesador).HasColumnName("TIP_PROCESADOR").HasMaxLength(50);
			Property(x => x.Memoria).HasColumnName("MEMORIA").HasMaxLength(50);
			Property(x => x.DesSo).HasColumnName("DES_SO").HasMaxLength(50);
			Property(x => x.VerSo).HasColumnName("VER_SO").HasMaxLength(20);
			Property(x => x.CapDisco).HasColumnName("CAP_DISCO").HasMaxLength(20);
			Property(x => x.TipDisco).HasColumnName("TIP_DISCO").HasMaxLength(20);
			Property(x => x.DesPuertoBalanza).HasColumnName("DES_PUERTO_BALANZA").HasMaxLength(50);
			Property(x => x.TipoCaja).HasColumnName("TIPO_CAJA").HasMaxLength(100);
			Property(x => x.Hostname).HasColumnName("HOSTNAME").HasMaxLength(50);
			Property(x => x.FechaInicioLising).HasColumnName("FECHA_INICIO_LISING");
			Property(x => x.FechaFinLising).HasColumnName("FECHA_FIN_LISING");
		}
	}
}
