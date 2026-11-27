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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NexoPad._01view
{
    /// <summary>
    /// Lógica de interacción para Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        private GestionEditor gestor;

        public Editor()
        {
            init();
            ListarFicheros();
        }

        public Editor(bool escanearHijas)
        {
            init();
            EscanearCarpeta(escanearHijas);
        }

        private void init()
        {
            InitializeComponent();
            gestor = new GestionEditor();
        }


        #region EVENTOS MENU

        private void TogglePropiedades_Click(object sender, RoutedEventArgs e) 
        {
            if (lbxPropiedades.IsVisible) lbxPropiedades.Visibility = Visibility.Collapsed;
            else lbxPropiedades.Visibility = Visibility.Visible;
        }

        #endregion


        #region EVENTOS EDITOR

        private void VerCarpeta_Click(object sender, RoutedEventArgs e)
        {
            string rutaCarpeta = tbxUbicacion.Text.Trim();
            if (!string.IsNullOrEmpty(rutaCarpeta) && Directory.Exists(rutaCarpeta))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = rutaCarpeta,
                    UseShellExecute = true
                });
            }
            else MessageBox.Show("La ruta es inválida o está vacía", "Error");
        }

        private void EscanearCarpetaUnica_Click(object sender, RoutedEventArgs e) => EscanearCarpeta(escanearHijas: false);

        private void EscanearCarpetaConHijas_Click(object sender, RoutedEventArgs e) => EscanearCarpeta(escanearHijas: true);

        private void ListaNotas_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarCamposYPropiedades();

        private void CopiarRuta_Click(object sender, RoutedEventArgs e) => Clipboard.SetText(tbxUbicacion.Text);

        private void PegarRuta_Click(object sender, RoutedEventArgs e) => tbxUbicacion.Text = Clipboard.GetText(); 

        private void BorrarRuta_Click(object sender, RoutedEventArgs e) => tbxUbicacion.Clear();

        private void RefrescarPropiedades_Click(object sender, RoutedEventArgs e) => ActualizarCamposYPropiedades();

        private void TextoEditorCambiado_TextChanged(object sender, TextChangedEventArgs e)
        {
            // if (cbxRefrescar.IsChecked == true) ActualizarCamposYPropiedades();

            NotaItem? notaSeleccionada = GetNotaSeleccionada();
            if (notaSeleccionada != null)
            {
                // mostrar en nombre algo que indique que la nota está por guardar
            }
        }

        private void CambiarRuta_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultado = PedirConfirmacion("¿Estás seguro de que deseas cambiar la ruta de este archivo?");
            if (resultado == MessageBoxResult.Yes)
            {
                if (lbxListaNotas.SelectedItem is not ListBoxItem item || item.Tag is not NotaItem nota)
                {
                    MessageBox.Show("Por favor, selecciona una nota primero.", "Atención");
                    return;
                }

                string nuevaRuta = tbxUbicacion.Text.Trim();
                int codigoProceso = gestor.CambiarRuta(nota, nuevaRuta);

                if (codigoProceso == -1) MessageBox.Show("La ruta no es válida o faltan permisos", "Error de traslado");
                else MessageBox.Show("Archivo movido con éxito", "Traslado exitoso");
            }
        }


        #endregion


        private MessageBoxResult PedirConfirmacion(string consulta)
        {
            return MessageBox.Show(
                consulta,
                "Confirmar acción",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No
            );
        }

        private NotaItem? GetNotaSeleccionada()
        {
            return lbxListaNotas.SelectedItem is ListBoxItem { Tag: NotaItem nota }
                ? nota
                : null;
        }

        private void EscanearCarpeta(bool escanearHijas)
        {
            int codigoProceso = gestor.SeleccionarYEscanearCarpeta(escanearHijas);
            if (codigoProceso == -1) MessageBox.Show("Ha habido un error o no hay archivos de texto en esta carpeta", "Error de escaneado");
            ListarFicheros();
        }

        private void ListarFicheros()
        {
            var notas = gestor.Notas;
            int cantidad = notas.Count();

            if (cantidad > 0) {
                lbxListaNotas.Items.Clear();
                notas.Sort((n1, n2) => n1.Nombre.CompareTo(n2.Nombre));
                
                foreach (var nota in notas)
                {
                    string nombreNota = string.IsNullOrEmpty(nota.Nombre)
                        ? "<sin título>"
                        : nota.Nombre;
                    var item = new ListBoxItem
                    {
                        Content = nombreNota,
                        Tag = nota
                    };
                    lbxListaNotas.Items.Add(item);
                }
            }
            txtCantidadNotas.Text = $"{cantidad} notas";
            ActualizarCamposYPropiedades();
        }

        private void ActualizarCamposYPropiedades()
        {
            NotaItem? notaSeleccionada = GetNotaSeleccionada();
            var campos = new Dictionary<TextBlock, (string Titulo, Func<string?> GetDato)>()
            {
                [txtTipo] = ("Tipo: ", () => notaSeleccionada?.Tipo),
                [txtCategoria] = ("Categoría: ", () => notaSeleccionada?.Categoria),
                [txtTamanyo] = ("Tamaño: ", () => notaSeleccionada?.Tamanyo),
                [txtCreado] = ("Creado: ", () => notaSeleccionada?.Creado),
                [txtModificado] = ("Modificado: ", () => notaSeleccionada?.Modificado),
                [txtUltimoAcceso] = ("Último acceso: ", () => notaSeleccionada?.UltimoAcceso)
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
                tbxUbicacion.Text = string.Empty;
                tbxEditorTexto.Text = string.Empty;
                return;
            }

            tbxUbicacion.Text = notaSeleccionada.Ubicacion;
            tbxEditorTexto.Text = notaSeleccionada.Contenido;
        }
    }
}
