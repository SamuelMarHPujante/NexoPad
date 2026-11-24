using NexoPad._02back;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NexoPad._01view
{
    /// <summary>
    /// Lógica de interacción para Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        private GestionEditor gestor;

        public Editor(bool escanear)
        {
            gestor = new GestionEditor();
            if (escanear) gestor.SeleccionarCarpeta();
            InitializeComponent();
            ListarFicheros();
        }


        #region EVENTOS

        private void AbrirCarpeta_Click(object sender, RoutedEventArgs e)
        {
            string rutaCarpeta = TxtUbicacion.Text.Trim();
            if (!String.IsNullOrEmpty(rutaCarpeta) && Directory.Exists(rutaCarpeta))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = rutaCarpeta,
                    UseShellExecute = true
                });
            }
            else MessageBox.Show("La ruta es inválida o está vacía", "Error");
        }

        private void EscanearCarpeta_Click(object sender, RoutedEventArgs e) 
        { 
            gestor.SeleccionarCarpeta();
            ListarFicheros();
        }

        private void ListaNotas_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarCamposYPropiedades();

        #endregion


        private void ListarFicheros()
        {
            var notas = gestor.Notas;
            int cantidad = notas.Count();

            if (cantidad > 0) {
                ListaNotas.Items.Clear();
                notas.Sort((n1, n2) => n1.Nombre.CompareTo(n2.Nombre));
                
                foreach (var nota in notas)
                {
                    var item = new ListBoxItem
                    {
                        Content = nota.Nombre,
                        Tag = nota
                    };
                    ListaNotas.Items.Add(item);
                }
            }
            TxtCantidadNotas.Text = $"{cantidad} notas";
            ActualizarCamposYPropiedades();
        }

        private void ActualizarCamposYPropiedades()
        {
            NotaItem? notaSeleccionada = ListaNotas.SelectedItem is ListBoxItem { Tag: NotaItem nota }
                    ? nota
                    : null;

            var campos = new Dictionary<TextBlock, (string Titulo, Func<string?> GetDato)>()
            {
                [TxtTipo] = ("Tipo: ", () => notaSeleccionada?.Tipo),
                [TxtCategoria] = ("Categoría: ", () => notaSeleccionada?.Categoria),
                [TxtTamanyo] = ("Tamaño: ", () => notaSeleccionada?.Tamanyo),
                [TxtCreado] = ("Creado: ", () => notaSeleccionada?.Creado),
                [TxtModificado] = ("Modificado: ", () => notaSeleccionada?.Modificado),
                [TxtUltimoAcceso] = ("Último acceso: ", () => notaSeleccionada?.UltimoAcceso)
            };

            foreach (var (textBlock, (titulo, getDato)) in campos)
            {
                string? dato = getDato();
                textBlock.Text = string.IsNullOrWhiteSpace(dato) 
                    ? titulo + "<sin datos>"
                    : titulo + dato;
            }

            if (notaSeleccionada is null)
            {
                TxtUbicacion.Text = string.Empty;
                EditorTexto.Text = string.Empty;
                return;
            }

            TxtUbicacion.Text = notaSeleccionada.Ubicacion;
            EditorTexto.Text = notaSeleccionada.Contenido;
        }
    }
}
