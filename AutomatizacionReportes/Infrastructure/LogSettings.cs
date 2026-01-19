namespace AutomatizacionReportes.Infrastructure
{
    public class LogSettings
    {
        public string Path { get; set; } = string.Empty;
        public string FilePrefix { get; set; } = "app";
        public string Level { get; set; } = "Info";
    }
}
