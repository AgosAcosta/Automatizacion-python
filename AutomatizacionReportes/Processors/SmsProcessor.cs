using AutomatizacionReportes.Models;
using AutomatizacionReportes.Utils;
using ClosedXML.Excel;
using System.Data;

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
                int unicos = ConteoUtils.ContarUnicos(tabla);

                resultados.Add(new SmsResultado
                {
                    Fecha = fechaProceso,
                    Banco = banco,
                    Tramo = tramo,
                    CantidadTotal = total,
                    CantidadUnica = unicos
                });
            }

            return resultados;
        }
        private static string ObtenerTramo(string path)
        {
            var nombre = Path.GetFileNameWithoutExtension(path);
            return nombre.Length >= 6 ? nombre[^6..].ToUpper() : "N/D";
        }
       
    }
}
