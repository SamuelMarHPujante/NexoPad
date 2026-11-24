using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NexoPad._02back
{
    class GestionEditor
    {
        public List<NotaItem> Notas { get; set; } = new();

        public GestionEditor()
        {
            
        }

        public void SeleccionarCarpeta()
        {
            try
            {
                var dialog = new OpenFolderDialog 
                { 
                    Title = "Escanear carpeta",
                    Multiselect = false
                };
                if (dialog.ShowDialog() == true) EscanearCarpeta(carpetaPath: dialog.FolderName);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error: algo salió mal");
            }
        }

        private void EscanearCarpeta(string carpetaPath)
        {
            var extensiones = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".txt", 
                ".md", 
                ".gitignore"
            };

            var archivosHallados = Directory.EnumerateFiles(carpetaPath, "*", SearchOption.AllDirectories)
                .Where(ruta => extensiones.Contains(Path.GetExtension(ruta)))
                .Select(NotaItem.DesdeArchivo)
                .ToList();

            if (archivosHallados.Count() > 0) this.Notas = archivosHallados;
            else MessageBox.Show("No hay archivos de texto en esta carpeta", "Error");
        }
    }
}
