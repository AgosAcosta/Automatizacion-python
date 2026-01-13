using System.Data;

namespace AutomatizacionReportes.Utils
{
    public static class DataTableUtils
    {
        public static void NormalizarColumnas(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                col.ColumnName = col.ColumnName.Trim().ToUpper();
            }
        }
    }
}
