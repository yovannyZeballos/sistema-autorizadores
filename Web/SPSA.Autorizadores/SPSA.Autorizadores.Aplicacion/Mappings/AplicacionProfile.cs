using AutoMapper;
using pe.oechsle.Entity;
using SPSA.Autorizadores.Aplicacion.DTO;
using SPSA.Autorizadores.Aplicacion.Features.Seguridad.Sistema.Commands;
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
			CreateMap<ListarSistemaDTO, SegSistema>().ReverseMap();
			CreateMap<CrearSistemaCommand, SegSistema>().ReverseMap();
			CreateMap<ActualizarSistemaCommand, SegSistema>().ReverseMap();
			CreateMap<SegSistema, ObtenerSistemaDTO>();

		}
	}
}
