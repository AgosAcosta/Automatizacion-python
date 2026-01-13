namespace AutomatizacionReportes.Models
{
    public class SmsResultado
    {
        public DateTime Fecha { get; set; }
        public string Banco { get; set; } = string.Empty;
        public string Tramo { get; set; } = string.Empty;
        public int CantidadTotal { get; set; }
        public int CantidadUnica { get; set; }
    }
}
