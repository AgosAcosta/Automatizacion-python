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
            // 1. Buscar archivos
            var archivos = _scanner.ObtenerArchivosWhatsapp();

            // 2. Procesar archivos
            var resultados = _processor.Procesar(archivos);

            // 3. Generar Excel (pisar)
            var archivoSalida = _writer.GenerarExcelWhatsapp(resultados);

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
