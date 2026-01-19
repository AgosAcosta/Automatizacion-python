using AutomatizacionReportes.Models;
using AutomatizacionReportes.Utils;
using ClosedXML.Excel;
using Microsoft.VisualBasic.FileIO;
using NPOI.Util;
using System.Data;

namespace AutomatizacionReportes.Processors
{
    public class MailProcessor
    {
        public List<MailResultado> Procesar(List<string> archivos)
        {
            var resultados = new List<MailResultado>();
            var fecha = DateTime.Today;

            foreach (var archivo in archivos)
            {
                var banco = FileNameUtils.ObtenerBanco(archivo);
                var tabla = FileReader.LeerArchivo(archivo);

                if (tabla == null || tabla.Rows.Count == 0)
                    continue;

                DataTableUtils.NormalizarColumnas(tabla);

                string? columnaCuit = tabla.Columns
                .Cast<DataColumn>()
                .Select(c => c.ColumnName.Trim().ToUpper())
                .FirstOrDefault(c => c == "CUIT");

                int total = tabla.Rows.Count;
                int unicos;

                if (columnaCuit != null)
                {
                    unicos = tabla.AsEnumerable()
                    .Select(r => r[columnaCuit]?.ToString()?.Trim())
                    .Where(v => !string.IsNullOrEmpty(v))
                    .Distinct()
                    .Count();
                }
                else
                {
                    unicos = total;
                }

                resultados.Add(new MailResultado
                {
                    Fecha = fecha,
                    Banco = banco,
                    CantidadTotal = total,
                    CantidadUnica = unicos 
                });
            }


            return resultados;
        }
       
    }
}

