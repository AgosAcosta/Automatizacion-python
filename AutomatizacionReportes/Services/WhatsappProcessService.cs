using AutomatizacionReportes.Models;

namespace AutomatizacionReportes.Services
{
    public class WhatsappProcessService
    {
        private readonly FileScanner _scanner;
        private readonly WhatsappProcessor _processor;
        private readonly ExcelWriter _writer;

        public WhatsappProcessService(
            FileScanner scanner,
            WhatsappProcessor processor,
            ExcelWriter writer)
        {
            _scanner = scanner;
            _processor = processor;
            _writer = writer;
        }

        public async Task<ProcessResult> EjecutarAsync()
        {
            var archivos = _scanner.ObtenerArchivosWhatsapp();
            var resultados = _processor.Procesar(archivos);
            var archivoSalida = _writer.GenerarExcelWhatsappHistorico(resultados);

            return new ProcessResult
            {
                Success = true,
                Message = "Proceso ejecutado correctamente",
                ArchivoGenerado = archivoSalida,
                FechaEjecucion = DateTime.Now
            };
        }
    }
}
