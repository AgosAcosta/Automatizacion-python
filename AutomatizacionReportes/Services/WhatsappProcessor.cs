using AutomatizacionReportes.Models;
using ClosedXML.Excel;
using System.Data;

namespace AutomatizacionReportes.Services
{
    public class WhatsappProcessor
    {
        public List<WhatsappResultado> Procesar(List<string> archivos)
        {
            var resultados = new List<WhatsappResultado>();
            var fechaProceso = DateTime.Today;

            foreach (var archivo in archivos)
            {
                var banco = ObtenerBancoDesdeNombre(archivo);
                var campania = ObtenerCampaniaDesdeNombre(archivo);

                var tabla = LeerArchivo(archivo);
                if (tabla == null)
                    continue;

                NormalizarColumnasUpper(tabla);

                if (!tabla.Columns.Contains("TEMPLATE"))
                {
                    resultados.Add(new WhatsappResultado
                    {
                        Fecha = fechaProceso,
                        Banco = banco,
                        Campania = campania,
                        Template = "No definido",
                        Cantidad = tabla.Rows.Count
                    });
                    continue;
                }

                var grupos = tabla.AsEnumerable()
                                   .GroupBy(r => r["TEMPLATE"]?.ToString()?.Trim() ?? "No definido");

                foreach (var g in grupos)
                {
                    resultados.Add(new WhatsappResultado
                    {
                        Fecha = fechaProceso,
                        Banco = banco,
                        Campania = campania,
                        Template = g.Key,
                        Cantidad = g.Count()
                    });
                }
            }

            return resultados;
        }

        // ------------------------
        // Helpers
        // ------------------------

        private static void NormalizarColumnasUpper(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
                col.ColumnName = col.ColumnName.Trim().ToUpper();
        }
      
        private static string ObtenerBancoDesdeNombre(string path)
        {
            var nombre = Path.GetFileName(path);

            // Ejemplo: 20260107_BSC_CAMP_WhatsApp.csv
            var partes = nombre.Split('_');
            return partes.Length > 1 ? partes[1].ToUpper() : "N/D";
        }

        private static string ObtenerCampaniaDesdeNombre(string path)
        {
            var nombre = Path.GetFileNameWithoutExtension(path);
            var sufijo = nombre.Length >= 2
                ? nombre[^2..].ToUpper()
                : string.Empty;

            return sufijo switch
            {
                "FI" => "REFI",
                "PP" => "RA",
                _ => sufijo
            };
        }

        private static DataTable? LeerArchivo(string path)
        {
            var ext = Path.GetExtension(path).ToLower();

            return ext switch
            {
                ".csv" => LeerCsv(path),
                ".xls" or ".xlsx" => LeerExcel(path),
                _ => null
            };
        }

        private static DataTable LeerCsv(string path)
        {
            var table = new DataTable();
            var lineas = File.ReadAllLines(path);

            if (lineas.Length == 0)
                return table;

            var headers = lineas[0].Split(';', ',', '\t');
            foreach (var h in headers)
                table.Columns.Add(h.Trim());

            for (int i = 1; i < lineas.Length; i++)
            {
                var valores = lineas[i].Split(';', ',', '\t');
                table.Rows.Add(valores);
            }

            return table;
        }

        private static DataTable LeerExcel(string path)
        {
            var table = new DataTable();

            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheets.First();

            bool primeraFila = true;

            foreach (var fila in ws.RowsUsed())
            {
                if (primeraFila)
                {
                    foreach (var celda in fila.Cells())
                        table.Columns.Add(celda.GetString());

                    primeraFila = false;
                }
                else
                {
                    var row = table.NewRow();
                    int i = 0;
                    foreach (var celda in fila.Cells(1, table.Columns.Count))
                    {
                        row[i++] = celda.Value.ToString();
                    }
                    table.Rows.Add(row);
                }
            }

            return table;
        }
    }
}
