using AutomatizacionReportes.Models;
using AutomatizacionReportes.Utils;
using ClosedXML.Excel;
using Microsoft.VisualBasic.FileIO;
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

                int total = tabla.Rows.Count;
                int unicos = ConteoUtils.ContarUnicos(tabla);

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

