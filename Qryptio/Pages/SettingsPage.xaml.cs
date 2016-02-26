using System.Windows.Controls;

namespace Qryptio.Pages
{
    public partial class SettingsPage : UserControl
    {
        private SettingsPageViewModel viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            viewModel = new SettingsPageViewModel(MainWindow.DefaultNavigator);
            DataContext = viewModel;
        }
    }
}
