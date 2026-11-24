using NexoPad._02back;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Editor()
        {
            gestor = new GestionEditor();
            gestor.SeleccionarCarpeta();
            InitializeComponent();
            ListarFicheros();
        }

        private void ListarFicheros()
        {
            var notas = gestor.Notas;
            int cantidad = notas.Count();

            ListaNotas.Items.Clear();
            if (cantidad > 0) {
                notas.Sort();
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
        }



        private void ListaNotas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaNotas.SelectedItem is not ListBoxItem { Tag: NotaItem notaSeleccionada }) 
                return;

            EditorTexto.Text = notaSeleccionada.Contenido;
            TxtTipo.Text = $"Tipo: {notaSeleccionada.Tipo}";
            TxtCategoria.Text = $"Categoría: {notaSeleccionada.Categoria}";
            TxtUbicacion.Text = $"Ubicación: {notaSeleccionada.Ubicacion}";
            TxtTamanyo.Text = $"Tamaño: {notaSeleccionada.Tamanyo}";
            TxtCreado.Text = $"Creado: {notaSeleccionada.Creado}";
            TxtModificado.Text = $"Modificado: {notaSeleccionada.Modificado}";
            TxtUltimoAcceso.Text = $"Último acceso: {notaSeleccionada.UltimoAcceso}";
        }
    }
}
