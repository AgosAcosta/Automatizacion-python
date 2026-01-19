using AutomatizacionReportes.Models;
using AutomatizacionReportes.Utils;
using ClosedXML.Excel;
using System.Data;
using System.Text.RegularExpressions;

namespace AutomatizacionReportes.Processors
{
    public class SmsProcessor
    {
        public List<SmsResultado> Procesar(List<string> archivos)
        {
            var resultados = new List<SmsResultado>();
            var fechaProceso = DateTime.Today;

            foreach (var archivo in archivos)
            {
                var banco = FileNameUtils.ObtenerBanco(archivo);
                var tramo = ObtenerTramo(archivo);

                var tabla = FileReader.LeerArchivo(archivo);
                if (tabla == null || tabla.Rows.Count == 0)
                    continue;

                DataTableUtils.NormalizarColumnas(tabla);

                int total = tabla.Rows.Count;
                int unicos = total;


                if (tabla.Columns.Contains("CUIT"))
                {
                    var listaDebug = tabla.AsEnumerable().Select(r => r["CUIT"]?.ToString()).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();
                    unicos = listaDebug.Count; 
                }

                    resultados.Add(new SmsResultado
                {
                    Fecha = fechaProceso,
                    Banco = banco,
                    Tramo = tramo,
                    CantidadTotal = total,
                    CantidadUnica = unicos
                });
            }

            string[] tramosEsperados = { "TRAMO1", "TRAMO2", "TRAMO3", "TRAMO4" };

            var resultadosCompletos = new List<SmsResultado>();

            foreach (var banco in resultados.Select(r => r.Banco).Distinct())
            {
                foreach (var tramo in tramosEsperados)
                {
                    var existente = resultados
                        .FirstOrDefault(r => r.Banco == banco && r.Tramo == tramo);

                    resultadosCompletos.Add(existente ?? new SmsResultado
                    {
                        Fecha = fechaProceso,
                        Banco = banco,
                        Tramo = tramo,
                        CantidadTotal = 0,
                        CantidadUnica = 0
                    });
                }
            }

            return resultadosCompletos;
        }
        private static string ObtenerTramo(string path)
        {
            var nombre = Path.GetFileNameWithoutExtension(path);
            return nombre.Length >= 6 ? nombre[^6..].ToUpper() : "N/D";
        }
       
    }
}
