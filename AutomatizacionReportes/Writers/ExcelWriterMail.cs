using AutomatizacionReportes.Models;
using ClosedXML.Excel;

namespace AutomatizacionReportes.Writers
{
    public class ExcelWriterMail
    {
        private readonly string _outputPath;

        public ExcelWriterMail(IConfiguration config)
        {
            _outputPath = config["OutputPaths:Mail"]
                ?? throw new Exception("Falta OutputPaths:Mail");
        }

        public string GenerarExcelHistorico(List<MailResultado> nuevos)
        {
            var final = Path.Combine(_outputPath, "Campañas_MAIL.xlsx");
            var temp = Path.Combine(_outputPath, "Campañas_MAIL.tmp.xlsx");

            var data = new List<MailResultado>();

            if (File.Exists(final))
                data.AddRange(LeerHistorico(final));

            var hoy = DateTime.Today;
            data = data.Where(d => d.Fecha.Date != hoy).ToList();
            data.AddRange(nuevos);

            Escribir(temp, data);

            if (File.Exists(final)) File.Delete(final);
            File.Move(temp, final);

            return final;
        }

        private List<MailResultado> LeerHistorico(string path)
        {
            var lista = new List<MailResultado>();
            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet("MAIL");

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                lista.Add(new MailResultado
                {
                    Fecha = row.Cell(1).GetDateTime(),
                    Banco = row.Cell(2).GetString(),
                    CantidadTotal = row.Cell(3).GetValue<int>(),
                    CantidadUnica = row.Cell(4).GetValue<int>()
                });
            }

            return lista;
        }

        private void Escribir(string path, List<MailResultado> data)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("MAIL");

            ws.Cell(1, 1).Value = "Fecha";
            ws.Cell(1, 2).Value = "Banco";
            ws.Cell(1, 3).Value = "Cantidad Total";
            ws.Cell(1, 4).Value = "Cantidad Única";
            ws.Range("A1:D1").Style.Font.Bold = true;

            int row = 2;
            foreach (var r in data.OrderBy(d => d.Fecha))
            {
                ws.Cell(row, 1).Value = r.Fecha;
                ws.Cell(row, 1).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Cell(row, 2).Value = r.Banco;
                ws.Cell(row, 3).Value = r.CantidadTotal;
                ws.Cell(row, 4).Value = r.CantidadUnica;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }
    }
}
