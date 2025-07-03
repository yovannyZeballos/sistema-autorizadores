using SPSA.Autorizadores.Dominio.Contrato.Repositorio;
using SPSA.Autorizadores.Infraestructura.Configuracion;
using SPSA.Autorizadores.Infraestructura.Repositorio;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPSA.Autorizadores.Infraestructura.Contexto
{
	[DbConfigurationType(typeof(NpgsqlConfiguration))]
	public class SGPContexto : DbContext, ISGPContexto
	{

		public SGPContexto() : base("SGP")
		{
			RepositorioSegSistema = new RepositorioSegSistema(this);
			RepositorioProcesoParametroEmpresa = new RepositorioProcesoParametroEmpresa(this);
			RepositorioSegUsuario = new RepositorioSegUsuario(this);
			RepositorioSegEmpresa = new RepositorioSegEmpresa(this);
			RepositorioSegCadena = new RepositorioSegCadena(this);
			RepositorioSegRegion = new RepositorioSegRegion(this);
			RepositorioSegZona = new RepositorioSegZona(this);
			RepositorioSegLocal = new RepositorioSegLocal(this);
			RepositorioSegPerfil = new RepositorioSegPerfil(this);
			RepositorioSegPerfilUsuario = new RepositorioSegPerfilUsuario(this);
			RepositorioSegMenu = new RepositorioSegMenu(this);
			RepositorioSegPerfilMenu = new RepositorioSegPerfilMenu(this);

			RepositorioMaeEmpresa = new RepositorioMaeEmpresa(this);
			RepositorioMaeCadena = new RepositorioMaeCadena(this);
			RepositorioMaeRegion = new RepositorioMaeRegion(this);
			RepositorioMaeZona = new RepositorioMaeZona(this);
			RepositorioMaeLocal = new RepositorioMaeLocal(this);
			RepositorioMaeCaja = new RepositorioMaeCaja(this);
            RepositorioMaeHorario = new RepositorioMaeHorario(this);

            RepositorioInvCajas = new RepositorioInvCajas(this);
			RepositorioInvTipoActivo = new RepositorioInvTipoActivo(this);

			RepositorioInventarioActivo = new RepositorioInventarioActivo(this);
			RepositorioApertura = new RepositorioApertura(this);

			RepositorioUbiDepartamento = new RepositorioUbiDepartamento(this);
			RepositorioUbiProvincia = new RepositorioUbiProvincia(this);
			RepositorioUbiDistrito = new RepositorioUbiDistrito(this);
			RepositorioProceso = new RepositorioProceso(this);
			RepositorioProcesoEmpresa = new RepositorioProcesoEmpresa(this);
			RepositorioMaeLocalAlterno = new RepositorioMaeLocalAlterno(this);
			RepositorioMonCierreLocal = new RepositorioMonCierreLocal(this);
			RepositorioTmpMonCierreLocal = new RepositorioTmpMonCierreLocal(this);
			RepositorioAutImpresion = new RepositorioAutImpresion(this);
			RepositorioProcesoParametro = new RepositorioProcesoParametro(this);

            RepositorioInvKardex = new RepositorioInvKardex(this);
            RepositorioInvKardexActivo = new RepositorioInvKardexActivo(this);
            RepositorioInvKardexLocal = new RepositorioInvKardexLocal(this);
			RepositorioMonCierreEOD = new RepositorioMonCierreEOD(this);
			RepositorioMonCierreEODHist = new RepositorioMonCierreEODHist(this);

            RepositorioMaeColaboradorExt = new RepositorioMaeColaboradorExt(this);
            RepositorioMaeColaboradorInt = new RepositorioMaeColaboradorInt(this);
            RepositorioMaePuesto = new RepositorioMaePuesto(this);
            RepositorioSolicitudUsuarioASR = new RepositorioSolicitudUsuarioASR(this);

            RepositorioCComSolicitudCab = new RepositorioCComSolicitudCab(this);
            RepositorioCComSolicitudDet = new RepositorioCComSolicitudDet(this);
            RepositorioMaeCodComercio = new RepositorioMaeCodComercio(this);

            RepositorioMdrBinesIzipay = new RepositorioMdrBinesIzipay(this);
            RepositorioMdrClasificacion = new RepositorioMdrClasificacion(this);
            RepositorioMdrOperador = new RepositorioMdrOperador(this);
            RepositorioMdrFactorIzipay = new RepositorioMdrFactorIzipay(this);
            RepositorioMdrPeriodo = new RepositorioMdrPeriodo(this);

			//this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s); //TODO: Borrar en producción
		}

		public IRepositorioSegSistema RepositorioSegSistema { get; private set; }
		public IRepositorioProcesoParametroEmpresa RepositorioProcesoParametroEmpresa { get; private set; }
		public IRepositorioSegUsuario RepositorioSegUsuario { get; private set; }
		public IRepositorioSegEmpresa RepositorioSegEmpresa { get; private set; }
		public IRepositorioSegCadena RepositorioSegCadena { get; private set; }
		public IRepositorioSegRegion RepositorioSegRegion { get; private set; }
		public IRepositorioSegZona RepositorioSegZona { get; private set; }
		public IRepositorioSegLocal RepositorioSegLocal { get; private set; }
		public IRepositorioSegPerfil RepositorioSegPerfil { get; private set; }
		public IRepositorioSegPerfilUsuario RepositorioSegPerfilUsuario { get; private set; }
		public IRepositorioSegMenu RepositorioSegMenu { get; private set; }
		public IRepositorioSegPerfilMenu RepositorioSegPerfilMenu { get; private set; }

		public IRepositorioMaeEmpresa RepositorioMaeEmpresa { get; private set; }
		public IRepositorioMaeCadena RepositorioMaeCadena { get; private set; }
		public IRepositorioMaeRegion RepositorioMaeRegion { get; private set; }
		public IRepositorioMaeZona RepositorioMaeZona { get; private set; }
		public IRepositorioMaeLocal RepositorioMaeLocal { get; private set; }
		public IRepositorioMaeCaja RepositorioMaeCaja { get; private set; }
		public IRepositorioMaeHorario RepositorioMaeHorario { get; private set; }
		public IRepositorioInvCajas RepositorioInvCajas { get; private set; }
		public IRepositorioInvTipoActivo RepositorioInvTipoActivo { get; private set; }

		public IRepositorioInventarioActivo RepositorioInventarioActivo { get; private set; }
		public IRepositorioApertura RepositorioApertura { get; private set; }

		public IRepositorioUbiDepartamento RepositorioUbiDepartamento { get; private set; }
		public IRepositorioUbiProvincia RepositorioUbiProvincia { get; private set; }
		public IRepositorioUbiDistrito RepositorioUbiDistrito { get; private set; }
		public IRepositorioProceso RepositorioProceso { get; private set; }
		public IRepositorioProcesoEmpresa RepositorioProcesoEmpresa { get; private set; }
		public IRepositorioMaeLocalAlterno RepositorioMaeLocalAlterno { get; private set; }
		public IRepositorioMonCierreLocal RepositorioMonCierreLocal { get; private set; }
		public IRepositorioTmpMonCierreLocal RepositorioTmpMonCierreLocal { get; private set; }
		public IRepositorioAutImpresion RepositorioAutImpresion { get; private set; }
		public IRepositorioProcesoParametro RepositorioProcesoParametro { get; private set; }

		public IRepositorioInvKardex RepositorioInvKardex { get; private set; }
		public IRepositorioInvKardexActivo RepositorioInvKardexActivo { get; private set; }
		public IRepositorioInvKardexLocal RepositorioInvKardexLocal { get; private set; }
		public IRepositorioMonCierreEOD RepositorioMonCierreEOD { get; private set; }
		public IRepositorioMonCierreEODHist RepositorioMonCierreEODHist { get; private set; }
		public IRepositorioMaeColaboradorExt RepositorioMaeColaboradorExt { get; private set; }
		public IRepositorioMaeColaboradorInt RepositorioMaeColaboradorInt { get; private set; }
		public IRepositorioMaePuesto RepositorioMaePuesto { get; private set; }
		public IRepositorioSolicitudUsuarioASR RepositorioSolicitudUsuarioASR { get; private set; }
		public IRepositorioCComSolicitudCab RepositorioCComSolicitudCab { get; private set; }
		public IRepositorioCComSolicitudDet RepositorioCComSolicitudDet { get; private set; }
		public IRepositorioMaeCodComercio RepositorioMaeCodComercio { get; private set; }

		public IRepositorioMdrBinesIzipay RepositorioMdrBinesIzipay { get; private set; }
		public IRepositorioMdrClasificacion RepositorioMdrClasificacion { get; private set; }
		public IRepositorioMdrOperador RepositorioMdrOperador { get; private set; }
		public IRepositorioMdrFactorIzipay RepositorioMdrFactorIzipay { get; private set; }
		public IRepositorioMdrPeriodo RepositorioMdrPeriodo { get; private set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new ProcesoParametroTypeConfiguration());
			modelBuilder.Configurations.Add(new ProcesoParametroEmpresaTypeConfiguration());
			modelBuilder.Configurations.Add(new SegSistemaTypeConfiguration());
			modelBuilder.Configurations.Add(new SegUsuarioTypeConfiguration());
			modelBuilder.Configurations.Add(new SegEmpresaTypeConfiguration());
			modelBuilder.Configurations.Add(new SegCadenaTypeConfiguration());
			modelBuilder.Configurations.Add(new SegRegionTypeConfiguration());
			modelBuilder.Configurations.Add(new SegZonaTypeConfiguration());
			modelBuilder.Configurations.Add(new SegLocalTypeConfiguration());
			modelBuilder.Configurations.Add(new SegPerfilTypeConfiguration());
			modelBuilder.Configurations.Add(new SegPerfilUsuarioTypeConfiguration());
			modelBuilder.Configurations.Add(new SegMenuTypeConfiguration());
			modelBuilder.Configurations.Add(new SegPerfilMenuTypeConfiguration());

			modelBuilder.Configurations.Add(new MaeEmpresaTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeCadenaTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeRegionTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeZonaTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeLocalTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeCajaTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeHorarioTypeConfiguration());

			modelBuilder.Configurations.Add(new InvActivoTypeConfiguration());
			modelBuilder.Configurations.Add(new AperturaTypeConfiguration());

			modelBuilder.Configurations.Add(new UbiDepartamentoTypeConfiguration());
			modelBuilder.Configurations.Add(new UbiProvinciaTypeConfiguration());
			modelBuilder.Configurations.Add(new UbiDistritoTypeConfiguration());
			modelBuilder.Configurations.Add(new InvCajasTypeConfiguration());
			modelBuilder.Configurations.Add(new InvTipoActivoTypeConfiguration());
			modelBuilder.Configurations.Add(new ProcesoTypeConfiguration());
			modelBuilder.Configurations.Add(new ProcesoEmpresaTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeLocalAlternoTypeConfiguration());
			modelBuilder.Configurations.Add(new MonCierreLocalTypeConfiguration());
			modelBuilder.Configurations.Add(new TmpMonCierreLocalTypeConfiguration());
			modelBuilder.Configurations.Add(new AutImpresionTypeConfiguracion());

            modelBuilder.Configurations.Add(new InvKardexTypeConfiguration());
            modelBuilder.Configurations.Add(new InvKardexActivoTypeConfiguration());
			modelBuilder.Configurations.Add(new InvKardexLocalTypeConfiguration());

			modelBuilder.Configurations.Add(new MonCierreEODTypeConfiguration());
			modelBuilder.Configurations.Add(new MonCierreEODHistTypeConfiguration());

			modelBuilder.Configurations.Add(new MaeColaboradorExtTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeColaboradorIntTypeConfiguration());
			modelBuilder.Configurations.Add(new MaePuestoTypeConfiguration());
			modelBuilder.Configurations.Add(new AsrSolicitudUsuarioTypeConfiguration());

			modelBuilder.Configurations.Add(new CcomSolicitudCabTypeConfiguration());
			modelBuilder.Configurations.Add(new CcomSolicitudDetTypeConfiguration());
			modelBuilder.Configurations.Add(new MaeCodComercioTypeConfiguration());

			modelBuilder.Configurations.Add(new MdrBinesIzipayTypeConfiguration());
			modelBuilder.Configurations.Add(new MdrClasificacionPorNombreTypeConfiguration());
			modelBuilder.Configurations.Add(new MdrClasificacionPorOperadorTypeConfiguration());
			modelBuilder.Configurations.Add(new MdrFactorIzipayTypeConfiguration());
			modelBuilder.Configurations.Add(new MdrOperadorTypeConfiguration());
			modelBuilder.Configurations.Add(new MdrPeriodoTypeConfiguration());

		}

		public bool GuardarCambios()
		{
			return SaveChanges() > 0;
		}

		public async Task<bool> GuardarCambiosAsync()
		{
			var filasAfectadas = await SaveChangesAsync();
			return filasAfectadas > 0;
		}

		public void Reestablecer()
		{
			ChangeTracker
			.Entries()
			.ToList()
			.ForEach(x => x.Reload());
		}

		public void Rollback()
		{
			ChangeTracker
				.Entries()
				.ToList()
				.ForEach(entry => entry.State = EntityState.Detached);
		}

		#region IDisposable Support
		public new void Dispose()
		{
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
