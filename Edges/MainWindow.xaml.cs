using Edges.View_Model;
using System.Windows;

namespace Edges
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var viewModel = new ViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
