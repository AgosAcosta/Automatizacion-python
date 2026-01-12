namespace AutomatizacionReportes.Models
{
    public class ProcessResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ArchivoGenerado { get; set; }
        public DateTime FechaEjecucion { get; set; }
    }
}
