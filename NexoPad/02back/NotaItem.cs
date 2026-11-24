using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NexoPad._02back
{
    class NotaItem
    {
        public string RutaCompleta { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;
        public string Categoria { get; set; } = "General";
        public string Ubicacion { get; set; } = string.Empty;
        public string Tamanyo { get; set; } = string.Empty;
        public string Creado { get; set; } = string.Empty;
        public string Modificado { get; set; } = string.Empty;
        public string UltimoAcceso { get; set; } = string.Empty;

        public static NotaItem DesdeArchivo(string ruta)
        {
            var info = new FileInfo(ruta);

            return new NotaItem
            {
                RutaCompleta = ruta,
                Nombre = Path.GetFileNameWithoutExtension(ruta), // o Path.GetFileName(ruta) si quieres la extensión
                Contenido = File.ReadAllText(ruta),
                Tipo = string.IsNullOrWhiteSpace(info.Extension) ? "Archivo" : info.Extension.ToUpperInvariant(),
                Categoria = "Nota",
                Ubicacion = info.DirectoryName ?? string.Empty,
                Tamanyo = $"{info.Length} bytes",
                Creado = info.CreationTime.ToString("g"),
                Modificado = info.LastWriteTime.ToString("g"),
                UltimoAcceso = info.LastAccessTime.ToString("g")
            };
        }
    }
}
