namespace AutomatizacionReportes.Services
{
    //Sirve para pisar los archivos cuando se ejecuta la tarea + ejecucion manual 
    public class ExecutionLockService
    {
        private static bool _isRunning = false;
        private static readonly object _lock = new();

        public bool TryStart()
        {
            lock (_lock)
            {
                if (_isRunning)
                    return false;

                _isRunning = true;
                return true;
            }
        }

        public void End()
        {
            lock (_lock)
            {
                _isRunning = false;
            }
        }
    }
}
