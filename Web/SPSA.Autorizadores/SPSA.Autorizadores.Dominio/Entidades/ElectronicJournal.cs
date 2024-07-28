namespace SPSA.Autorizadores.Dominio.Entidades
{
	public class ElectronicJournal
	{
        public int Terminal { get; set; }
        public int Operator { get; set; }
        public int Transaction { get; set; }
        public string Date { get; set; }
        public string Hour { get; set; }
        public int ActionCode { get; set; }
        public int ActionSubCode { get; set; }
        public int Store { get; set; }
        public string TrxData { get; set; }
    }
}
