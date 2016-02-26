using System.Windows.Controls;

namespace Qryptio.Pages
{
    public partial class AboutPage : UserControl
    {
        private AboutPageViewModel viewModel;

        public AboutPage()
        {
            InitializeComponent();

            viewModel = new AboutPageViewModel(MainWindow.DefaultNavigator);
            DataContext = viewModel;
        }
    }
}
