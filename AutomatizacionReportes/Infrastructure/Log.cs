namespace AutomatizacionReportes.Infrastructure
{
    public static class Log
    {
        private static readonly object _lock = new();

        private static LogSettings _settings = new();
        private static bool _initialized = false;

        public static void Init(IConfiguration config)
        {
            _settings = config.GetSection("Logging").Get<LogSettings>()
                        ?? throw new Exception("Sección Logging no configurada");

            if (string.IsNullOrWhiteSpace(_settings.Path))
                throw new Exception("Logging:Path no configurado");

            if (!Directory.Exists(_settings.Path))
                Directory.CreateDirectory(_settings.Path);

            _initialized = true;
        }

        private static string LogFile
        {
            get
            {
                if (!_initialized)
                    throw new Exception("Logger no inicializado");

                var fileName =
                    $"{_settings.FilePrefix}_{DateTime.Today:yyyyMMdd}.log";

                return System.IO.Path.Combine(_settings.Path, fileName);
            }
        }

        public static void Info(string message)
            => Write("INFO", message);

        public static void Error(string message, Exception ex)
            => Write("ERROR", $"{message} | {ex}");

        private static void Write(string level, string message)
        {
            lock (_lock)
            {
                File.AppendAllText(
                    LogFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}"
                );
            }
        }
    }

}
