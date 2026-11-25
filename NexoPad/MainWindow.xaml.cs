using NexoPad._01view;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NexoPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<Button, Func<Window>> _pantallas;

        public MainWindow()
        {
            InitializeComponent();
            _pantallas = new Dictionary<Button, Func<Window>>()
            {
                [btnAbrirEditor] = () => new Editor(escanear: false),
                [btnEscanearYEditar] = () => new Editor(escanear: true, escanearHijas: false),
                [btnEscanearTodasYEditar] = () => new Editor(escanear: true, escanearHijas: true),
                // PARA MÁS PANTALLAS A FUTURO
            };
        }

        private void Menus_Click(object sender, RoutedEventArgs e)
        {
            Button emisor = (Button)sender;
            if (_pantallas.ContainsKey(emisor))
            {
                this.Hide();
                Window pantalla = _pantallas[emisor].Invoke();
                pantalla.ShowDialog();
                this.Show();
            }
        }

    }
}