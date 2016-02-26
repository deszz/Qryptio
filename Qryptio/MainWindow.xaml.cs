using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.Controls;

using Qryptio.Wpf;

namespace Qryptio
{
    public partial class MainWindow : MetroWindow, IContentWindow
    {
        public static Navigator DefaultNavigator
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow).Navigator;
            }
        }

        public ContentControl ContentControl
        {
            get
            {
                return contentControl;
            }
        }
        public Navigator Navigator;

        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            Navigator = new Navigator(this);

            viewModel = new MainWindowViewModel(Navigator);
            DataContext = viewModel;
        }

        public void EnableCloseButton()
        {
            IsCloseButtonEnabled = true;
        }

        public void DisableCloseButton()
        {
            IsCloseButtonEnabled = false;
        }
    }
}
