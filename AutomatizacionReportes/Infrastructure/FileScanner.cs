namespace AutomatizacionReportes.Infrastructure
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
        private readonly string[] _filtrosSms =
        {
            "SMS_TRAMO"
        };

        private readonly string[] _filtrosMail =
        {
            "MAILS_ENVIO1"
        };

        private readonly string[] _filtrosMailAsignacion =
        {
            "MAILS_ASIGNACION_ENVIO"
        };
        public FileScanner(IConfiguration config)
        {
            _config = config;
        }

        private List<string> ObtenerArchivos(
        string configKey, string[] filtros)
        {
            var resultado = new List<string>();

            var paths = _config.GetSection($"InputPaths:{configKey}").Get<string[]>();
            if (paths == null || paths.Length == 0)
                return resultado;

            var fechaHoy = DateTime.Today.ToString("yyyyMMdd");

            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                    continue;

                foreach (var archivo in Directory.GetFiles(path))
                {
                    var nombre = Path.GetFileName(archivo);

                    if (!nombre.StartsWith(fechaHoy))
                        continue;

                    if (filtros.Any(f => nombre.Contains(f)))
                        resultado.Add(archivo);
                }
            }

            return resultado;
        }

        public List<string> ObtenerArchivosWhatsapp()
        {
            return ObtenerArchivos("Whatsapp", _filtrosWhatsapp);
        }

        public List<string> ObtenerArchivosSms()
        {
            return ObtenerArchivos("Sms", _filtrosSms);
        }

        public List<string> ObtenerArchivosMail()
        {
            return ObtenerArchivos("Mail", _filtrosMail);
        }

        public List<string> ObtenerArchivosMailAsignacion()
        {
            return ObtenerArchivos("MailAsignacion", _filtrosMailAsignacion);
        }
    }
}
