using AutomatizacionReportes.Infrastructure;
using AutomatizacionReportes.Models;
using AutomatizacionReportes.Processors;
using AutomatizacionReportes.Writers;

namespace AutomatizacionReportes.Services
{
    public class MailProcessService
    {
        private readonly FileScanner _scanner;
        private readonly MailProcessor _processor;
        private readonly ExcelWriterMail _writer;

        public MailProcessService(
            FileScanner scanner,
            MailProcessor processor,
            ExcelWriterMail writer)
        {
            _scanner = scanner;
            _processor = processor;
            _writer = writer;
        }

        public Task<ProcessResult> EjecutarAsync()
        {
            Log.Info("MAIL - Inicio de ejecución");

            try
            {
                var archivos = _scanner.ObtenerArchivosMail();
                Log.Info($"MAIL - Archivos encontrados: {archivos.Count}");

                var resultados = _processor.Procesar(archivos);
                var archivo = _writer.GenerarExcelHistorico(resultados);

                return Task.FromResult(new ProcessResult
                {
                    Success = true,
                    Message = "Proceso MAIL ejecutado correctamente",
                    ArchivoGenerado = archivo,
                    FechaEjecucion = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Log.Error("MAIL - Error durante la ejecución", ex);
                throw;
            }
            finally
            {
                Log.Info("MAIL - Fin de ejecución");
            }

        }
    }
}
