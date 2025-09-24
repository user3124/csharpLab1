using databinding0.ViewModels;
using System.Windows;

namespace databinding0
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
