namespace AutomatizacionReportes.Utils
{
    public static class FileNameUtils
    {
        public static string ObtenerBanco(string path)
        {
            var nombre = Path.GetFileName(path);
            var partes = nombre.Split('_');
            return partes.Length > 1 ? partes[1].ToUpper() : "N/D";
        }

        public static string ObtenerCampania(string path)
        {
            var nombre = Path.GetFileNameWithoutExtension(path);
            var sufijo = nombre.Length >= 2
                ? nombre[^2..].ToUpper()
                : string.Empty;

            return sufijo switch
            {
                "FI" => "REFI",
                "PP" => "RA",
                _ => sufijo
            };
        }
    }
}
