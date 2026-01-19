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
                var banco = ObtenerBancoDesdeNombre(archivo);
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

        //Metodo diferente que el resto: El nombre del archivo tiene la fecha y el nombre junto. El resto es con _
        private static string ObtenerBancoDesdeNombre(string path)
        {
            var nombre = Path.GetFileNameWithoutExtension(path);

            if (nombre.Length < 11)
                return "N/D";

            return nombre.Substring(8, 3).ToUpper();


        }

    }
}

