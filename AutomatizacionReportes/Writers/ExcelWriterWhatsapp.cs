using AutomatizacionReportes.Models;
using ClosedXML.Excel;

namespace AutomatizacionReportes.Writers
{

    public class ExcelWriterWhatsapp
    {
        private readonly string _outputPath;

        public ExcelWriterWhatsapp(IConfiguration config)
        {
            _outputPath = config["OutputPaths:Whatsapp"]
                ?? throw new Exception("No está configurado la ruta de salida");
        }

        public string GenerarExcelWhatsappHistorico(List<WhatsappResultado> nuevosResultados)
        {
            var tempFile = Path.Combine(_outputPath, "Campañas_Whatsapp.tmp.xlsx");
            var finalFile = Path.Combine(_outputPath, "Campañas_Whatsapp.xlsx");

            var resultadosFinales = new List<WhatsappResultado>();

            if (File.Exists(finalFile))
            {
                resultadosFinales.AddRange(LeerHistorico(finalFile));
            }

            var fechaHoy = DateTime.Today.Date;

            resultadosFinales = resultadosFinales
                .Where(r => r.Fecha.Date != fechaHoy)
                .ToList();

            resultadosFinales.AddRange(nuevosResultados);

            EscribirExcel(tempFile, resultadosFinales);

            if (File.Exists(finalFile))
                File.Delete(finalFile);

            File.Move(tempFile, finalFile);

            return finalFile;
        }

        private List<WhatsappResultado> LeerHistorico(string path)
        {
            var lista = new List<WhatsappResultado>();

            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet("Whatsapp");

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                lista.Add(new WhatsappResultado
                {
                    Fecha = row.Cell(1).GetDateTime(),
                    Banco = row.Cell(2).GetString(),
                    Campania = row.Cell(3).GetString(),
                    Template = row.Cell(4).GetString(),
                    Cantidad = row.Cell(5).GetValue<int>()
                });
            }

            return lista;
        }

        private void EscribirExcel(string path, List<WhatsappResultado> datos)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Whatsapp");

            ws.Cell(1, 1).Value = "Fecha";
            ws.Cell(1, 2).Value = "Banco";
            ws.Cell(1, 3).Value = "Campaña";
            ws.Cell(1, 4).Value = "Template";
            ws.Cell(1, 5).Value = "Cantidad";

            ws.Range("A1:E1").Style.Font.Bold = true;

            int row = 2;
            foreach (var r in datos.OrderBy(d => d.Fecha))
            {
                ws.Cell(row, 1).Value = r.Fecha;
                ws.Cell(row, 1).Style.DateFormat.Format = "dd/MM/yyyy";

                ws.Cell(row, 2).Value = r.Banco;
                ws.Cell(row, 3).Value = r.Campania;
                ws.Cell(row, 4).Value = r.Template;
                ws.Cell(row, 5).Value = r.Cantidad;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }
    }
}
