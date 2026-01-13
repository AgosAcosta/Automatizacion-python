using AutomatizacionReportes.Models;
using AutomatizacionReportes.Utils;
using System.Data;

namespace AutomatizacionReportes.Processors
{
    public class MailAsignacionProcessor
    {
        public List<MailAsignacionResultado> Procesar(List<string> archivos)
        {
            var resultados = new List<MailAsignacionResultado>();
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

                resultados.Add(new MailAsignacionResultado
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
