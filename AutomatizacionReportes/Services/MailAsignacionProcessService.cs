using AutomatizacionReportes.Infrastructure;
using AutomatizacionReportes.Models;
using AutomatizacionReportes.Processors;
using AutomatizacionReportes.Writers;

namespace AutomatizacionReportes.Services
{
    public class MailAsignacionProcessService
    {
        private readonly FileScanner _scanner;
        private readonly MailAsignacionProcessor _processor;
        private readonly ExcelWriterMailAsignacion _writer;

        public MailAsignacionProcessService(
            FileScanner scanner,
            MailAsignacionProcessor processor,
            ExcelWriterMailAsignacion writer)
        {
            _scanner = scanner;
            _processor = processor;
            _writer = writer;
        }

        public Task<ProcessResult> EjecutarAsync()
        {
            Log.Info("MAIL ASIGNACIÓN - Inicio de ejecución");

            try
            {
                var archivos = _scanner.ObtenerArchivosMailAsignacion();
                Log.Info($"MAIL ASIGNACIÓN - Archivos encontrados: {archivos.Count}");

                var resultados = _processor.Procesar(archivos);
                var archivo = _writer.GenerarExcelHistorico(resultados);

                return Task.FromResult(new ProcessResult
                {
                    Success = true,
                    Message = "Proceso MAIL ASIGNACIÓN ejecutado correctamente",
                    ArchivoGenerado = archivo,
                    FechaEjecucion = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Log.Error("MAIL ASIGNACIÓN - Error durante la ejecución", ex);
                throw;
            }
            finally
            {
                Log.Info("MAIL ASIGNACIÓN - Fin de ejecución");
            }
        }
    }
}
