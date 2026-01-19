using AutomatizacionReportes.Infrastructure;
using AutomatizacionReportes.Models;
using AutomatizacionReportes.Processors;
using AutomatizacionReportes.Writers;

namespace AutomatizacionReportes.Services
{
    public class WhatsappProcessService
    {
        private readonly FileScanner _scanner;
        private readonly WhatsappProcessor _processor;
        private readonly ExcelWriterWhatsapp _writer;

        public WhatsappProcessService(
            FileScanner scanner,
            WhatsappProcessor processor,
            ExcelWriterWhatsapp writer)
        {
            _scanner = scanner;
            _processor = processor;
            _writer = writer;
        }

        public async Task<ProcessResult> EjecutarAsync()
        {
            Log.Info("WHATSAPP - Inicio de ejecución");

            try
            {
                var archivos = _scanner.ObtenerArchivosWhatsapp();
                Log.Info($"WHATSAPP - Archivos encontrados: {archivos.Count}");

                var resultados = _processor.Procesar(archivos);
                var archivoSalida = _writer.GenerarExcelWhatsappHistorico(resultados);

                return new ProcessResult
                {
                    Success = true,
                    Message = "Proceso WHATSAPP ejecutado correctamente",
                    ArchivoGenerado = archivoSalida,
                    FechaEjecucion = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Log.Error("WHATSAPP - Error durante la ejecución", ex);
                throw;
            }
            finally
            {
                Log.Info("WHATSAPP - Fin de ejecución");
            }
        }
    }
}
