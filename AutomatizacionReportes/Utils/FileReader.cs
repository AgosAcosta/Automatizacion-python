using System.Data;

namespace AutomatizacionReportes.Utils
{
    public static class FileReader
    {
        public static DataTable? LeerArchivo(string path)
        {
            var ext = Path.GetExtension(path).ToLower();

            try
            {
                if (ext == ".xls")
                {
                    try
                    {
                        path = ExcelConverter.ConvertXlsToXlsx(path);
                        return LeerExcel(path);
                    }
                    catch (NPOI.POIFS.FileSystem.NotOLE2FileException)
                    {
                        return LeerCsv(path);
                    }
                }

                return ext switch
                {
                    ".csv" => LeerCsv(path),
                    ".xlsx" => LeerExcel(path),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error leyendo archivo '{Path.GetFileName(path)}': {ex.Message}", ex);
            }
        }

        private static DataTable LeerCsv(string path)
        {
            var table = new DataTable();

            using var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(path);
            parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            parser.SetDelimiters(",", ";", "\t");
            parser.HasFieldsEnclosedInQuotes = true;

            if (!parser.EndOfData)
            {
                var headers = parser.ReadFields();
                if (headers != null)
                    foreach (var h in headers)
                        table.Columns.Add(h.Trim());
            }

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null) continue;

                var row = table.NewRow();
                for (int i = 0; i < table.Columns.Count && i < fields.Length; i++)
                    row[i] = fields[i];

                table.Rows.Add(row);
            }

            return table;
        }

        private static DataTable LeerExcel(string path)
        {
            var table = new DataTable();
            using var wb = new ClosedXML.Excel.XLWorkbook(path);
            var ws = wb.Worksheets.First();

            bool header = true;
            foreach (var row in ws.RowsUsed())
            {
                if (header)
                {
                    foreach (var cell in row.Cells())
                        table.Columns.Add(cell.GetString());
                    header = false;
                }
                else
                {
                    var dr = table.NewRow();
                    for (int i = 0; i < table.Columns.Count; i++)
                        dr[i] = row.Cell(i + 1).GetValue<string>();
                    table.Rows.Add(dr);
                }
            }

            return table;
        }
    }
}
