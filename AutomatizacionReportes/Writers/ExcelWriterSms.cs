using AutomatizacionReportes.Models;
using ClosedXML.Excel;

namespace AutomatizacionReportes.Writers
{
    public class ExcelWriterSms
    {
        private readonly string _outputPath;

        public ExcelWriterSms(IConfiguration config)
        {
            _outputPath = config["OutputPaths:Sms"]
                ?? throw new Exception("No está configurado la ruta de salida");
        }

        public string GenerarExcelHistorico(List<SmsResultado> nuevos)
        {
            var temp = Path.Combine(_outputPath, "Campañas_SMS.tmp.xlsx");
            var final = Path.Combine(_outputPath, "Campañas_SMS.xlsx");

            var data = new List<SmsResultado>();

            if (File.Exists(final))
                data.AddRange(LeerHistorico(final));

            var hoy = DateTime.Today.Date;
            data = data.Where(d => d.Fecha.Date != hoy).ToList();
            data.AddRange(nuevos);

            Escribir(temp, data);

            if (File.Exists(final))
                File.Delete(final);

            File.Move(temp, final);
            return final;
        }

        private List<SmsResultado> LeerHistorico(string path)
        {
            var lista = new List<SmsResultado>();
            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet("SMS");

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                lista.Add(new SmsResultado
                {
                    Fecha = row.Cell(1).GetDateTime(),
                    Banco = row.Cell(2).GetString(),
                    Tramo = row.Cell(3).GetString(),
                    CantidadTotal = row.Cell(4).GetValue<int>(),
                    CantidadUnica = row.Cell(5).GetValue<int>()
                });
            }

            return lista;
        }

        private void Escribir(string path, List<SmsResultado> data)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("SMS");

            ws.Cell(1, 1).Value = "Fecha";
            ws.Cell(1, 2).Value = "Banco";
            ws.Cell(1, 3).Value = "Tramo";
            ws.Cell(1, 4).Value = "Cantidad Total";
            ws.Cell(1, 5).Value = "Cantidad Única";

            ws.Range("A1:E1").Style.Font.Bold = true;

            int row = 2;
            foreach (var r in data.OrderBy(d => d.Fecha))
            {
                ws.Cell(row, 1).Value = r.Fecha;
                ws.Cell(row, 1).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Cell(row, 2).Value = r.Banco;
                ws.Cell(row, 3).Value = r.Tramo;
                ws.Cell(row, 4).Value = r.CantidadTotal;
                ws.Cell(row, 5).Value = r.CantidadUnica;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }
    }
}
