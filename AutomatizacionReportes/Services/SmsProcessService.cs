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
            var archivos = _scanner.ObtenerArchivosSms();
            var resultados = _processor.Procesar(archivos);
            var archivoSalida = _writer.GenerarExcelHistorico(resultados);

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
