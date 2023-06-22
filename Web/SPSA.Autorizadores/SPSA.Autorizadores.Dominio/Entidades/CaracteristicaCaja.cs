namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class CaracteristicaCaja
	{
        public int Id { get; private set; }
        public string Descripcion { get; private set; }
        public int Tipo { get; private set; }

		public CaracteristicaCaja(int id, string descripcion)
		{
			Id = id;
			Descripcion = descripcion;
		}
	}
}
