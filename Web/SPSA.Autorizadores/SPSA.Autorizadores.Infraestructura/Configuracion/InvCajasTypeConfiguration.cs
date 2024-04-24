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

			Property(x => x.CodEmpresa).HasColumnName("COD_EMPRESA");
			Property(x => x.CodCadena).HasColumnName("COD_CADENA");
			Property(x => x.CodRegion).HasColumnName("COD_REGION");
			Property(x => x.CodZona).HasColumnName("COD_ZONA");
			Property(x => x.CodLocal).HasColumnName("COD_LOCAL");
			Property(x => x.NumCaja).HasColumnName("NUM_CAJA");
			Property(x => x.CodActivo).HasColumnName("COD_ACTIVO");
			Property(x => x.CodModelo).HasColumnName("COD_MODELO");
			Property(x => x.CodSerie).HasColumnName("COD_SERIE");
			Property(x => x.NumAdenda).HasColumnName("NUM_ADENDA");
			Property(x => x.FecGarantia).HasColumnName("FEC_GARANTIA");
			Property(x => x.TipEstado).HasColumnName("TIP_ESTADO");
			Property(x => x.TipProcesador).HasColumnName("TIP_PROCESADOR");
			Property(x => x.Memoria).HasColumnName("MEMORIA");
			Property(x => x.DesSo).HasColumnName("DES_SO");
			Property(x => x.VerSo).HasColumnName("VER_SO");
			Property(x => x.CapDisco).HasColumnName("CAP_DISCO");
			Property(x => x.TipDisco).HasColumnName("TIP_DISCO");
			Property(x => x.DesPuertoBalanza).HasColumnName("DES_PUERTO_BALANZA");
			Property(x => x.TipoCaja).HasColumnName("TIPO_CAJA");
			Property(x => x.Hostname).HasColumnName("HOSTNAME");
			Property(x => x.FechaInicioLising).HasColumnName("FECHA_INICIO_LISING");
			Property(x => x.FechaFinLising).HasColumnName("FECHA_FIN_LISING");
		}
	}
}
