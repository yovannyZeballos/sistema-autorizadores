using AutoMapper;
using pe.oechsle.Entity;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Aperturas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cadenas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Cajas.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.Commands;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresExt.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.ColaboradoresInt.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Correo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Empresas.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Horarios.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.InventarioActivo.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioCaja.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.Commands;
using SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Locales.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Puestos.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Regiones.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Menu.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Perfil.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Commands;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Usuario.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.Commands;
using SPSA.Autorizadores.Aplicacion.Features.SolicitudUsuarioASR.DTOs;
using SPSA.Autorizadores.Aplicacion.Features.Zona.Commands;
using SPSA.Autorizadores.Dominio.Entidades;

namespace SPSA.Autorizadores.Aplicacion.Mappings
{
	public class AplicacionProfile : Profile
	{
		public AplicacionProfile()
		{
			CreateMap<SegUsuario, UsuarioDTO>().ReverseMap();
			CreateMap<SegPermiso, PermisoDTO>().ReverseMap();
			CreateMap<Empresa, EmpresaDTO>().ReverseMap();
			CreateMap<Local, LocalDTO>().ReverseMap();
			CreateMap<pe.oechsle.ex.Entity.Local, LocalDTO>().ReverseMap();
			CreateMap<Colaborador, ColaboradorDTO>().ReverseMap();
			CreateMap<Autorizador, AutorizadorDTO>().ReverseMap();

			CreateMap<pe.oechsle.ex.Entity.Opcion, PermisoDTO>()
			   .ForMember(dest => dest.TipoOpc, opt => opt.MapFrom(src => src.tipo))
			   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
			   .ForMember(dest => dest.Nivel, opt => opt.MapFrom(src => src.nivelAnidamiento))
			   .ForMember(dest => dest.CodNiv2, opt => opt.MapFrom(src => src.posicion))
			   .ForMember(dest => dest.DesPermiso, opt => opt.MapFrom(src => src.descripcion))
			   .ForMember(dest => dest.AsigTip, opt => opt.MapFrom(src => src.accesoPorTipo))
			   .ForMember(dest => dest.AsigUsr, opt => opt.MapFrom(src => src.accesoPorUsuario));

			CreateMap<pe.oechsle.ex.Entity.Aplicacion, AplicacionDTO>()
			   .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.version))
			   .ForMember(dest => dest.Permisos, opt => opt.MapFrom(src => src.Opciones));

			CreateMap<Usuario, UsuarioDTO>()
				.ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.nombreCompleto))
				.ForMember(dest => dest.Aplicacion, opt => opt.MapFrom(src => src.App))
				.ForMember(dest => dest.CodEmpleado, opt => opt.MapFrom(src => src.codigoEmpleado));

			CreateMap<SovosFormato, FormatoDTO>().ReverseMap();
			CreateMap<SovosLocal, SovosLocalDTO>().ReverseMap();
			CreateMap<LocalOfiplan, LocalOfiplanDTO>().ReverseMap();
			CreateMap<SovosCajaInventario, SovosCajaInventarioDTO>()
				.ForMember(dest => dest.FechaApertura, opt => opt.MapFrom(src => src.FechaApertura == null ? "" : src.FechaApertura.Value.ToString("dd/MM/yyyy")))
				.ForMember(dest => dest.FechaAsignacion, opt => opt.MapFrom(src => src.FechaAsignacion == null ? "" : src.FechaAsignacion.Value.ToString("dd/MM/yyyy")))
				.ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion == null ? "" : src.FechaCreacion.Value.ToString("dd/MM/yyyy HH:mm:ss")))
				.ForMember(dest => dest.FechaLising, opt => opt.MapFrom(src => src.FechaLising == null ? "" : src.FechaLising.Value.ToString("dd/MM/yyyy")))
				.ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaModificacion == null ? "" : src.FechaModificacion.Value.ToString("dd/MM/yyyy HH:mm:ss")))
				.ReverseMap();

			CreateMap<InventarioTipo, InventarioTipoDTO>().ReverseMap();
			CreateMap<InventarioServidor, InventarioServidorDTO>()
				.ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion == null ? "" : src.FechaCreacion.Value.ToString("dd/MM/yyyy  HH:mm:ss")))
				.ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaModificacion == null ? "" : src.FechaModificacion.Value.ToString("dd/MM/yyyy  HH:mm:ss")))
				.ReverseMap();

			CreateMap<ListBox, ListBoxDTO>();
			CreateMap<ListarSistemaDTO, Seg_Sistema>().ReverseMap();
			CreateMap<CrearSistemaCommand, Seg_Sistema>().ReverseMap();
			CreateMap<ActualizarSistemaCommand, Seg_Sistema>().ReverseMap();
			CreateMap<Seg_Sistema, ObtenerSistemaDTO>();
			CreateMap<ListarUsuarioDTO, Seg_Usuario>().ReverseMap();
			CreateMap<CrearUsuarioCommand, Seg_Usuario>().ReverseMap();
			CreateMap<ActualizarUsuarioCommand, Seg_Usuario>().ReverseMap();
			CreateMap<Mae_Empresa, ListarEmpresaDTO>().ReverseMap();
			CreateMap<Seg_Empresa, ListarEmpresaDTO>()
				.ForPath(dest => dest.NomEmpresa, opt => opt.MapFrom(src => src.Mae_Empresa == null ? "" : src.Mae_Empresa.NomEmpresa));
			CreateMap<Seg_Cadena, ListarCadenaDTO>()
				.ForPath(dest => dest.NomCadena, opt => opt.MapFrom(src => src.Mae_Cadena == null ? "" : src.Mae_Cadena.NomCadena));
			CreateMap<Seg_Region, ListarRegionDTO>()
				.ForPath(dest => dest.NomRegion, opt => opt.MapFrom(src => src.Mae_Region == null ? "" : src.Mae_Region.NomRegion));
			//CreateMap<Seg_Local, ListarLocalDTO>()
			//	.ForPath(dest => dest.NomLocal, opt => opt.MapFrom(src => src.Mae_Local == null ? "" : src.Mae_Local.NomLocal));
            CreateMap<Seg_Local, ListarLocalDTO>()
                .ForMember(dest => dest.NomLocal, opt => opt.MapFrom(src => src.Mae_Local == null ? "" : src.Mae_Local.NomLocal))
                .ForMember(dest => dest.CodLocalAlterno, opt => opt.MapFrom(src => src.Mae_Local == null ? "" : src.Mae_Local.CodLocalAlterno));

            CreateMap<Mae_Cadena, ListarCadenaDTO>();
			CreateMap<Mae_Region, ListarRegionDTO>();
			CreateMap<Mae_Zona, ListarZonaDTO>();
			CreateMap<Seg_Zona, ListarZonaDTO>()
				.ForPath(dest => dest.NomZona, opt => opt.MapFrom(src => src.Mae_Zona == null ? "" : src.Mae_Zona.NomZona));
			CreateMap<Mae_Local, ListarLocalDTO>();
			CreateMap<ListarPerfilDTO, Seg_Perfil>().ReverseMap();
			CreateMap<CrearPerfilCommand, Seg_Perfil>().ReverseMap();
			CreateMap<ActualizarPerfilCommand, Seg_Perfil>().ReverseMap();
			CreateMap<CrearMenuCommand, Seg_Menu>();
			CreateMap<Seg_Menu, ListarMenuDTO>();
			CreateMap<ActualizarMenuCommand, Seg_Menu>();
			CreateMap<Seg_Usuario, ObtenerUsuarioDTO>();

            #region <--TABLA_MAESTROS_SGP-->

            CreateMap<MaeEmpresaDTO, Mae_Empresa>().ReverseMap();
            CreateMap<MaeCadenaDTO, Mae_Cadena>().ReverseMap();
            CreateMap<MaeRegionDTO, Mae_Region>().ReverseMap();
            CreateMap<MaeZonaDTO, Mae_Zona>().ReverseMap();
            CreateMap<MaeLocalDTO, Mae_Local>().ReverseMap();
            CreateMap<MaeCajaDTO, Mae_Caja>().ReverseMap();
            CreateMap<AperturaDTO, Apertura>().ReverseMap();

            CreateMap<ListarMaeEmpresaDTO, Mae_Empresa>().ReverseMap();
            CreateMap<ObtenerMaeEmpresaDTO, Mae_Empresa>().ReverseMap();
            CreateMap<CrearMaeEmpresaCommand, Mae_Empresa>().ReverseMap();
            CreateMap<ActualizarMaeEmpresaCommand, Mae_Empresa>().ReverseMap();

            CreateMap<ListarMaeCadenaDTO, Mae_Cadena>().ReverseMap();
            CreateMap<ObtenerMaeCadenaDTO, Mae_Cadena>().ReverseMap();
            CreateMap<CrearMaeCadenaCommand, Mae_Cadena>().ReverseMap();
            CreateMap<ActualizarMaeCadenaCommand, Mae_Cadena>().ReverseMap();

            CreateMap<ListarMaeRegionDTO, Mae_Region>().ReverseMap();
            CreateMap<ObtenerMaeRegionDTO, Mae_Region>().ReverseMap();
            CreateMap<CrearMaeRegionCommand, Mae_Region>().ReverseMap();
            CreateMap<ActualizarMaeRegionCommand, Mae_Region>().ReverseMap();

            CreateMap<ListarMaeZonaDTO, Mae_Zona>().ReverseMap();
            CreateMap<ObtenerMaeZonaDTO, Mae_Zona>().ReverseMap();
            CreateMap<CrearMaeZonaCommand, Mae_Zona>().ReverseMap();
            CreateMap<ActualizarMaeZonaCommand, Mae_Zona>().ReverseMap();

            CreateMap<ListarMaeLocalDTO, Mae_Local>().ReverseMap();
            CreateMap<ObtenerMaeLocalDTO, Mae_Local>().ReverseMap();
            CreateMap<CrearMaeLocalCommand, Mae_Local>().ReverseMap();
            CreateMap<ActualizarMaeLocalCommand, Mae_Local>().ReverseMap();

            CreateMap<ListarMaeCajaDTO, Mae_Caja>().ReverseMap();
            CreateMap<ObtenerMaeCajaDTO, Mae_Caja>().ReverseMap();
            CreateMap<CrearMaeCajaCommand, Mae_Caja>().ReverseMap();
            CreateMap<ActualizarMaeCajaCommand, Mae_Caja>().ReverseMap();

            CreateMap<MaeHorarioDTO, Mae_Horario>().ReverseMap();
            CreateMap<ListarMaeHorarioDTO, Mae_Horario>().ReverseMap();
            CreateMap<ObtenerMaeHorarioDTO, Mae_Horario>().ReverseMap();
            CreateMap<CrearMaeHorarioCommand, Mae_Horario>().ReverseMap();
            CreateMap<ActualizarMaeHorarioCommand, Mae_Horario>().ReverseMap();

            CreateMap<InvTipoActivoDTO, InvTipoActivo>().ReverseMap();

            CreateMap<ListarInvActivoDTO, Inv_Activo>().ReverseMap();
            CreateMap<InvActivoDTO, Inv_Activo>().ReverseMap();
            CreateMap<ObtenerInvActivoDTO, Inv_Activo>().ReverseMap();
            CreateMap<CrearInvActivoCommand, Inv_Activo>().ReverseMap();
            CreateMap<ActualizarInvActivoCommand, Inv_Activo>().ReverseMap();
            CreateMap<EliminarInvActivoCommand, Inv_Activo>().ReverseMap();

            CreateMap<ListarInvCajaDTO, InvCajas>().ReverseMap();
            CreateMap<InvCajaDTO, InvCajas>().ReverseMap();
            CreateMap<ObtenerInvCajaDTO, InvCajas>().ReverseMap();
            CreateMap<CrearInvCajaCommand, InvCajas>().ReverseMap();
            CreateMap<ActualizarInvCajaCommand, InvCajas>().ReverseMap();
            CreateMap<EliminarInvCajaCommand, InvCajas>().ReverseMap();

            CreateMap<ListarAperturaDTO, Apertura>().ReverseMap();
            CreateMap<ObtenerAperturaDTO, Apertura>().ReverseMap();
            CreateMap<CrearAperturaCommand, Apertura>().ReverseMap();
            CreateMap<ActualizarAperturaCommand, Apertura>().ReverseMap();

            CreateMap<ListarUbiDepartamentoDTO, UbiDepartamento>().ReverseMap();
            CreateMap<ListarUbiProvinciaDTO, UbiProvincia>().ReverseMap();
            CreateMap<ListarUbiDistritoDTO, UbiDistrito>().ReverseMap();
            CreateMap<ObtenerUbiDistritoDTO, UbiDistrito>().ReverseMap();

            #endregion

            #region <--INV_KARDEX-->

            CreateMap<ListarInvKardexActivoDTO, InvKardexActivo>().ReverseMap();
            CreateMap<InvKardexActivoDTO, InvKardexActivo>().ReverseMap();
            CreateMap<CrearInvKardexActivoCommand, InvKardexActivo>().ReverseMap();
            CreateMap<ActualizarInvKardexActivoCommand, InvKardexActivo>().ReverseMap();

            CreateMap<ListarInvKardexDTO, InvKardex>().ReverseMap();
            CreateMap<InvKardexDTO, InvKardex>().ReverseMap();
            CreateMap<CrearInvKardexCommand, InvKardex>().ReverseMap();
            CreateMap<ActualizarInvKardexCommand, InvKardex>().ReverseMap();

            CreateMap<ListarInvKardexLocalDTO, InvKardexLocal>().ReverseMap();
            CreateMap<InvKardexLocalDTO, InvKardexLocal>().ReverseMap();
            CreateMap<CrearInvKardexLocalCommand, InvKardexLocal>().ReverseMap();
            CreateMap<ActualizarInvKardexLocalCommand, InvKardexLocal>().ReverseMap();

            #endregion

            #region <--COLABORADOR_EXTERNO-->
            CreateMap<ListarMaeColaboradorExtDTO, Mae_ColaboradorExt>().ReverseMap();
            CreateMap<ObtenerMaeColaboradorExtDTO, Mae_ColaboradorExt>().ReverseMap();
            CreateMap<MaeColaboradorExtDTO, Mae_ColaboradorExt>().ReverseMap();
            CreateMap<CrearMaeColaboradorExtCommand, Mae_ColaboradorExt>().ReverseMap();
            CreateMap<ActualizarMaeColaboradorExtCommand, Mae_ColaboradorExt>().ReverseMap();
            #endregion

            #region <--COLABORADOR_INTERNO-->
            CreateMap<ListarMaeColaboradorIntDTO, Mae_ColaboradorInt>().ReverseMap();
            CreateMap<ObtenerMaeColaboradorIntDTO, Mae_ColaboradorInt>().ReverseMap();
            CreateMap<MaeColaboradorIntDTO, Mae_ColaboradorInt>().ReverseMap();
            //CreateMap<CrearMaeColaboradorExtCommand, Mae_ColaboradorExt>().ReverseMap();
            //CreateMap<ActualizarMaeColaboradorExtCommand, Mae_ColaboradorExt>().ReverseMap();
            #endregion

            #region <--MAE_PUESTO-->
            CreateMap<ListarMaePuestoDTO, Mae_Puesto>().ReverseMap();
            CreateMap<ObtenerMaePuestoDTO, Mae_Puesto>().ReverseMap();
            CreateMap<MaePuestoDTO, Mae_Puesto>().ReverseMap();
            CreateMap<ActualizarMaePuestoCommand, Mae_Puesto>().ReverseMap();
            #endregion

            //CreateMap<EnviarCorreoCommand, ASR_SolicitudUsuario>().ReverseMap();

            #region <--ASR_SOLICITUD_USUARIO-->
            CreateMap<ListarSolictudUsuarioDTO, ASR_SolicitudUsuario>().ReverseMap();
            //CreateMap<ObtenerMaeColaboradorExtDTO, ASR_SolicitudUsuario>().ReverseMap();
            //CreateMap<MaeColaboradorExtDTO, ASR_SolicitudUsuario>().ReverseMap();
            CreateMap<CrearSolicitudUsuarioCommand, ASR_SolicitudUsuario>().ReverseMap();
            CreateMap<EliminarSolicitudUsuarioCommand, ASR_SolicitudUsuario>().ReverseMap();
            //CreateMap<ActualizarMaeColaboradorExtCommand, ASR_SolicitudUsuario>().ReverseMap();
            #endregion

            CreateMap<MonCierreEOD, MonCierreEODHist>();


		}
	}
}
