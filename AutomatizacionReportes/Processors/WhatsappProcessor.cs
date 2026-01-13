using AutomatizacionReportes.Models;
using AutomatizacionReportes.Utils;
using ClosedXML.Excel;
using System.Data;

namespace AutomatizacionReportes.Processors
{
    public class WhatsappProcessor
    {
        public List<WhatsappResultado> Procesar(List<string> archivos)
        {
            var resultados = new List<WhatsappResultado>();
            var fechaProceso = DateTime.Today;

            foreach (var archivo in archivos)
            {
                var banco = FileNameUtils.ObtenerBanco(archivo);
                var campania = FileNameUtils.ObtenerCampania(archivo);

                var tabla = FileReader.LeerArchivo(archivo);
                if (tabla == null)
                    continue;

                DataTableUtils.NormalizarColumnas(tabla);

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
    }
}
