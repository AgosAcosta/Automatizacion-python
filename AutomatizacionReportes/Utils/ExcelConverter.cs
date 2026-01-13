using ClosedXML.Excel;
using NPOI.HSSF.UserModel;

namespace AutomatizacionReportes.Utils
{
    public static class ExcelConverter
    {
        public static string ConvertXlsToXlsx(string xlsPath)
        {
            var xlsxPath = Path.ChangeExtension(xlsPath, ".xlsx");

            using var fs = new FileStream(xlsPath, FileMode.Open, FileAccess.Read);
            var hssfWorkbook = new HSSFWorkbook(fs);
            var sheet = hssfWorkbook.GetSheetAt(0);

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet(sheet.SheetName);

            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;

                for (int j = 0; j < row.LastCellNum; j++)
                {
                    var cell = row.GetCell(j);
                    if (cell == null) continue;

                    ws.Cell(i + 1, j + 1).Value = cell.ToString();
                }
            }

            wb.SaveAs(xlsxPath);
            return xlsxPath;
        }
    }
}
