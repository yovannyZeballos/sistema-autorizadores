namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class ListBox
	{
		public string Nombre { get; private set; }
		public int Opcion { get; private set; }

		public ListBox(string nombre, int opcion)
		{
			Nombre = nombre;
			Opcion = opcion;
		}
	}
}
