using AutomatizacionReportes.Models;
using ClosedXML.Excel;

namespace AutomatizacionReportes.Services
{

    public class ExcelWriter
    {
        private readonly string _outputPath;

        public ExcelWriter(IConfiguration config)
        {
            _outputPath = config["OutputPaths:Whatsapp"];
        }

        public string GenerarExcelWhatsapp(List<WhatsappResultado> resultados)
        {
            var tempFile = Path.Combine(_outputPath, "Campañas_Whatsapp.tmp.xlsx");
            var finalFile = Path.Combine(_outputPath, "Campañas_Whatsapp.xlsx");

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.AddWorksheet("Whatsapp");

                ws.Cell(1, 1).Value = "Fecha";
                ws.Cell(1, 2).Value = "Banco";
                ws.Cell(1, 3).Value = "Campaña";
                ws.Cell(1, 4).Value = "Template";
                ws.Cell(1, 5).Value = "Cantidad";

                ws.Range("A1:E1").Style.Font.Bold = true;

                int row = 2;
                foreach (var r in resultados)
                {
                    ws.Cell(row, 1).Value = r.Fecha;
                    ws.Cell(row, 2).Value = r.Banco;
                    ws.Cell(row, 3).Value = r.Campania;
                    ws.Cell(row, 4).Value = r.Template;
                    ws.Cell(row, 5).Value = r.Cantidad;
                    row++;
                }

                ws.Columns().AdjustToContents();
                workbook.SaveAs(tempFile);
            }

            if (File.Exists(finalFile))
                File.Delete(finalFile);

            File.Move(tempFile, finalFile);

            return finalFile;
        }
    }
}
