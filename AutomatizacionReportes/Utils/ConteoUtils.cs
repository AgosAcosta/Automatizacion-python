using System.Data;

namespace AutomatizacionReportes.Utils
{
    public static class ConteoUtils
    {
        public static int ContarUnicos(DataTable table)
        {
            if (table.Columns.Contains("CUIT"))
            {
                return table.AsEnumerable()
                    .Select(r => r["CUIT"]?.ToString()?.Trim())
                    .Where(v => !string.IsNullOrEmpty(v))
                    .Distinct()
                    .Count();
            }

            if (table.Columns.Contains("DNI"))
            {
                return table.AsEnumerable()
                    .Select(r => r["DNI"]?.ToString()?.Trim())
                    .Where(v => !string.IsNullOrEmpty(v))
                    .Distinct()
                    .Count();
            }

            return table.Rows.Count;
        }
    }
}
