using AutomatizacionReportes.Infrastructure;
using AutomatizacionReportes.Models;
using AutomatizacionReportes.Processors;
using AutomatizacionReportes.Writers;

namespace AutomatizacionReportes.Services
{
    public class SmsProcessService
    {
        private readonly FileScanner _scanner;
        private readonly SmsProcessor _processor;
        private readonly ExcelWriterSms _writer;

        public SmsProcessService(
            FileScanner scanner,
            SmsProcessor processor,
            ExcelWriterSms writer)
        {
            _scanner = scanner;
            _processor = processor;
            _writer = writer;
        }

        public async Task<ProcessResult> EjecutarAsync()
        {
            Log.Info("SMS - Inicio de ejecución");

            try
            {
                var archivos = _scanner.ObtenerArchivosSms();
                Log.Info($"SMS - Archivos encontrados: {archivos.Count}");

                var resultados = _processor.Procesar(archivos);
                var archivoSalida = _writer.GenerarExcelHistorico(resultados);

                return new ProcessResult
                {
                    Success = true,
                    Message = "Proceso SMS ejecutado correctamente",
                    ArchivoGenerado = archivoSalida,
                    FechaEjecucion = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Log.Error("SMS - Error durante la ejecución", ex);
                throw;
            }
            finally
            {
                Log.Info("SMS - Fin de ejecución");
            }
        }

    }
}
