
namespace SPSA.Autorizadores.Dominio.Entidades
{
    public class Puesto
    {
        public string CodEmpresa { get; private set; }
        public string CodPuesto { get; private set; }
        public string Nombre { get; private set; }
        public string IndAutorizador { get; private set; }
        public string IndCajero { get; private set; }
        public string IndAutoCajero { get; private set; }
        public string IndAutoAutorizador { get; private set; }

		public Puesto(string codEmpresa, string codPuesto, string indAutorizador, string indCajero, string indAutoCajero, string indAutoAutorizador)
		{

			CodEmpresa = codEmpresa;
			CodPuesto = codPuesto;
			IndAutorizador = indAutorizador;
			IndAutorizador = indAutorizador;
			IndCajero = indCajero;
			IndAutoCajero = indAutoCajero;
			IndAutoAutorizador = indAutoAutorizador;
		}
    }
}
