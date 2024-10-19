using SPSA.Autorizadores.Dominio.Entidades;
using System;

namespace SPSA.Autorizadores.Aplicacion.Features.InventarioKardex.DTOs
{
    public class ListarInvKardexDTO
    {
        public int Id { get; set; }
        public string Kardex { get; set; }
        public DateTime Fecha { get; set; }
        public string Guia { get; set; }

        //public string ActivoId { get; set; }
        public string Serie { get; set; }
       
        //public string Origen { get; set; }
        //public string Destino { get; set; }
        public string Tk { get; set; }
        public int Cantidad { get; set; }
        public string TipoStock { get; set; }
        public string Oc { get; set; }
        public string Sociedad { get; set; }
        public InvKardexLocal Origen { get; set; }

        public string OrigenLocal
        {
            get
            {
                return Origen?.NomLocal;
            }
        }

        public InvKardexLocal Destino { get; set; }

        public string DestinoLocal
        {
            get
            {
                return Destino?.NomLocal;
            }
        }
        public InvKardexActivo InvKardexActivo { get; set; }

        public string ActivoId
        {
            get
            {
                return InvKardexActivo?.Id;
            }
        }

        public string ActivoModelo
        {
            get
            {
                return InvKardexActivo?.Modelo;
            }
        }

        public string ActivoDescripcion
        {
            get
            {
                return InvKardexActivo?.Descripcion;
            }
        }

        public string ActivoMarca
        {
            get
            {
                return InvKardexActivo?.Marca;
            }
        }

        public string ActivoArea
        {
            get
            {
                return InvKardexActivo?.Area;
            }
        }

        public string ActivoTipo
        {
            get
            {
                return InvKardexActivo?.Tipo;
            }
        }
    }
}
