using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace NexoPad._02back
{
    class GestionEditor
    {
        public List<NotaItem> Notas { get; set; } = new();

        public GestionEditor()
        {
            
        }

        public void SeleccionarCarpeta(bool escanearHijas)
        {
            try
            {
                var dialog = new OpenFolderDialog 
                { 
                    Title = "Escanear carpeta",
                    Multiselect = false
                };
                if (dialog.ShowDialog() == true) EscanearCarpeta(carpetaPath: dialog.FolderName, escanearHijas);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error: algo salió mal");
            }
        }

        private void EscanearCarpeta(string carpetaPath, bool escanearHijas)
        {
            var extensiones = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".txt", 
                ".md", 
                ".gitignore"
            };

            SearchOption opcionEscaneo = escanearHijas ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var archivosHallados = Directory.EnumerateFiles(carpetaPath, "*", opcionEscaneo)
                .Where(ruta => extensiones.Contains(Path.GetExtension(ruta)))
                .Select(NotaItem.DesdeArchivo)
                .ToList();

            if (archivosHallados.Count() > 0) this.Notas = archivosHallados;
            else MessageBox.Show("No hay archivos de texto en esta carpeta", "Error");
        }

        public int CambiarRuta(NotaItem nota, string rutaDestino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nota.RutaCompleta) || string.IsNullOrWhiteSpace(rutaDestino))
                    return -1;

                string nombreArchivo = Path.GetFileName(nota.RutaCompleta);
                string rutaArchivoDestino = rutaDestino;

                if (Directory.Exists(rutaDestino) || string.IsNullOrEmpty(Path.GetExtension(rutaDestino)))
                    rutaArchivoDestino = Path.Combine(rutaDestino, nombreArchivo);

                if (string.Equals(
                        Path.GetFullPath(nota.RutaCompleta),
                        Path.GetFullPath(rutaArchivoDestino),
                        StringComparison.OrdinalIgnoreCase)
                )
                    return 0;

                string? carpetaDestino = Path.GetDirectoryName(rutaArchivoDestino);
                if (!string.IsNullOrEmpty(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);

                File.Move(nota.RutaCompleta, rutaArchivoDestino, overwrite: true);
                nota.RutaCompleta = rutaArchivoDestino;
                nota.Ubicacion = carpetaDestino ?? string.Empty;
                return 0;
            }
            catch (IOException error)
            {
                Debug.WriteLine("Error de ficheros en CambiarRuta(): " + error.Message);
                return -1;
            }
            catch (UnauthorizedAccessException error)
            {
                Debug.WriteLine("Error de acceso en CambiarRuta(): " + error.Message);
                return -1;
            }
        }
    }
}
