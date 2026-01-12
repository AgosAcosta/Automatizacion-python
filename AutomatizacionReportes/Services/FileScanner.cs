namespace AutomatizacionReportes.Services
{
    public class FileScanner
    {
        private readonly IConfiguration _config;

        private readonly string[] _filtrosWhatsapp =
        {
            "CAMP_WhatsApp",
            "CAMP_WhatsApp_RB",
            "CAMP_WhatsApp_RM",
            "CAMP_WhatsApp_Refi"
        };

        public FileScanner(IConfiguration config)
        {
            _config = config;
        }

        public List<string> ObtenerArchivosWhatsapp()
        {
            var resultado = new List<string>();

            var paths = _config.GetSection("InputPaths:Whatsapp").Get<string[]>();
            if (paths == null || paths.Length == 0)
                return resultado;

            var fechaHoy = DateTime.Now.ToString("yyyyMMdd");

            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                    continue;

                var archivos = Directory.GetFiles(path);

                foreach (var archivo in archivos)
                {
                    var nombre = Path.GetFileName(archivo);

                    if (!nombre.StartsWith(fechaHoy))
                        continue;

                    if (_filtrosWhatsapp.Any(f => nombre.Contains(f)))
                        resultado.Add(archivo);
                }
            }

            return resultado;
        }
    }
}
