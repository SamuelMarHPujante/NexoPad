using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NexoPad._02back
{
    class GestionEditor
    {
        public List<NotaItem> Notas { get; set; } = new();
        private readonly HashSet<string> EXTENSIONES = new HashSet<string>(comparer: StringComparer.OrdinalIgnoreCase)
        {
            ".txt",
            ".md",
            ".gitignore"
        };

        #region LECTURA Y ESCRITURA

        private int CrearNota(/* Parámetros por debatir */)
        {
            return -1;
        }

        private int GuardarNota(NotaItem notaPorGuardar)
        {
            return -1;
        }

        private int BorrarNota(NotaItem notaPorBorrar)
        {
            return -1;
        }

        #endregion


        public int SeleccionarYEscanearCarpeta(bool escanearHijas)
        {
            try
            {
                var dialog = new OpenFolderDialog 
                { 
                    Title = "Escanear carpeta",
                    Multiselect = false
                };

                if (dialog.ShowDialog() == true) 
                {
                    string carpetaPath = dialog.FolderName;
                    SearchOption opcionEscaneo = escanearHijas ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var archivosHallados = Directory.EnumerateFiles(carpetaPath, "*", opcionEscaneo)
                        .Where(ruta => EXTENSIONES.Contains(Path.GetExtension(ruta)))
                        .Select(NotaItem.DesdeArchivo)
                        .ToList();

                    if (archivosHallados.Count() > 0) 
                    { 
                        this.Notas = archivosHallados;
                        return 0;
                    }

                    Debug.WriteLine("No hay archivos de texto en la carpeta escaneada", "Error");
                    return -1;
                }
                return 0;
            }
            catch (Exception error)
            {
                Debug.WriteLine("Algo salió mal al escanear: " + error.Message, "Error");
                return -1;
            }
        }

        public int CambiarRuta(NotaItem notaPorMover, string carpetaDestino)
        {
            try
            {
                string rutaOriginal = notaPorMover.RutaCompleta;
                if (string.IsNullOrWhiteSpace(rutaOriginal) || string.IsNullOrWhiteSpace(carpetaDestino))
                {
                    Debug.WriteLine("Una de las rutas es null o está vacia", "Error");
                    return -1;
                }

                if (!File.Exists(rutaOriginal))
                {
                    Debug.WriteLine("El archivo original no existe: " + rutaOriginal, "Error");
                    return -1;
                }

                string nombreArchivo = Path.GetFileName(rutaOriginal);
                string rutaArchivoDestino = Path.Combine(carpetaDestino, nombreArchivo);

                if (string.Equals(
                        Path.GetFullPath(rutaOriginal),
                        Path.GetFullPath(rutaArchivoDestino),
                        StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("La ruta original y final son idénticas", "Warning");
                    return 0; 
                }

                string? ubicacionFinal = Path.GetDirectoryName(rutaArchivoDestino);
                if (!string.IsNullOrEmpty(ubicacionFinal)) 
                    Directory.CreateDirectory(ubicacionFinal);

                File.Move(rutaOriginal, rutaArchivoDestino, overwrite: true);
                notaPorMover.RutaCompleta = rutaArchivoDestino;
                notaPorMover.CarpetaPadre = ubicacionFinal ?? string.Empty;
                return 0;
            }
            catch (IOException error)
            {
                Debug.WriteLine("Error de ficheros al trasladar: " + error.Message, "Error");
                return -1;
            }
            catch (UnauthorizedAccessException error)
            {
                Debug.WriteLine("Error de permisos al trasladar: " + error.Message, "Error");
                return -1;
            }
        }
    }
}
