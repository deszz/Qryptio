using System;
using System.Windows.Controls;
using System.Windows.Input;

using Qryptio.Controls;

namespace Qryptio.Pages
{
    public partial class FilePage : UserControl
    {
        private FilePageViewModel viewModel;

        public FilePage(string fileName)
        {
            InitializeComponent();

            viewModel = new FilePageViewModel(MainWindow.DefaultNavigator, fileName);
            viewModel.InvalidPasswordEntered += (sender, e) => passwordBox.HighlightPasswordBox();

            // Binding within XAML isn't working and i don't even want to know why
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals(nameof(viewModel.OperationActive)))
                    passwordBox.IsEnabled = !viewModel.OperationActive;
            };

            DataContext = viewModel;
        }


        #region Events

        private void PasswordChangedHandler(object sender, EventArgs e)
        {
            viewModel.Password = ((QryptioPasswordBox)sender).Password;
        }

        private void PageMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }

        #endregion
    }
}
